var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};

// Глобальные переменные
let connection;
let authToken;
let mediaRecorder;
let audioChunks = [];
let audioStream;
let isRecording = false;
let audioSubject;

// Элементы визуализации
const canvas = document.getElementById('visualizer');
const canvasCtx = canvas.getContext('2d');
let audioContext;
let analyser;
let dataArray;

// Инициализация приложения
function initializeApp() {
    authToken = localStorage.getItem('authToken');
    if (authToken) {
        showAppContent();
        initializeConnection();
    } else {
        showLoginForm();
    }
}

// Показать форму логина
function showLoginForm() {
    document.getElementById('loginSection').classList.remove('hidden');
    document.getElementById('appContent').classList.add('hidden');
}

// Показать основное приложение
function showAppContent() {
    document.getElementById('loginSection').classList.add('hidden');
    document.getElementById('appContent').classList.remove('hidden');
    document.getElementById('userInfo').textContent = localStorage.getItem('userEmail') || 'Authenticated';
}

// Инициализация SignalR соединения с токеном - СОГЛАСНО ПРИМЕРУ
function initializeConnection() {
    console.log("🔄 Initializing SignalR connection with token:", authToken);

    // СОЗДАЕМ соединение с accessTokenFactory
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/interview", {
            accessTokenFactory: () => {
                console.log("🔑 accessTokenFactory called, token exists:", !!authToken);
                return authToken;
            }
        })
        .configureLogging(signalR.LogLevel.Debug)
        .build();

    setupConnectionHandlers();

    // ЗАПУСКАЕМ соединение ТОЛЬКО если есть токен
    if (authToken) {
        startConnection();
    }
}

// Настройка обработчиков соединения
function setupConnectionHandlers() {
    connection.onclose((error) => {
        console.log("Connection closed: ", error);
        document.getElementById("connectionStatus").textContent = "Disconnected";
        document.getElementById("connectionStatus").className = "disconnected";
        setTimeout(startConnection, 5000);
    });

    connection.onreconnecting((error) => {
        console.log("Reconnecting: ", error);
        document.getElementById("connectionStatus").textContent = "Reconnecting...";
    });

    connection.onreconnected((connectionId) => {
        console.log("Reconnected: ", connectionId);
        document.getElementById("connectionStatus").textContent = "Connected";
        document.getElementById("connectionStatus").className = "connected";
    });
}

// Функция логина - ИСПРАВЛЕННАЯ ПОСЛЕДОВАТЕЛЬНОСТЬ
function login(email, password) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            console.log("🔐 Attempting login for:", email);

            const response = yield fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            console.log("📡 Login response status:", response.status);

            if (!response.ok) {
                const errorText = yield response.text();
                console.error("❌ Login failed:", errorText);
                throw new Error(errorText || 'Login failed');
            }

            const token = yield response.text();
            console.log("✅ Login successful, token received");

            // Сохраняем токен
            authToken = token;
            localStorage.setItem('authToken', token);
            localStorage.setItem('userEmail', email);

            document.getElementById('loginStatus').textContent = 'Login successful!';
            document.getElementById('loginStatus').style.color = 'green';

            // Инициализируем приложение ПОСЛЕ получения токена
            showAppContent();

            // Переинициализируем соединение с новым токеном
            initializeConnection();

            return true;
        } catch (error) {
            console.error('❌ Login error:', error);
            document.getElementById('loginStatus').textContent = `Login failed: ${error.message}`;
            document.getElementById('loginStatus').style.color = 'red';
            return false;
        }
    });
}

// Функция логаута
function logout() {
    authToken = null;
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');

    if (connection) {
        connection.stop();
    }

    showLoginForm();
    addMessage("Logged out successfully", 'info-message');
}

function startConnection() {
    return __awaiter(this, void 0, void 0, function* () {
        if (!connection || !authToken) return;

        try {
            yield connection.start();
            console.log("✅ Connected successfully to SignalR");
            document.getElementById("connectionStatus").textContent = "Connected";
            document.getElementById("connectionStatus").className = "connected";
        } catch (err) {
            console.error("❌ Connection failed: ", err);
            document.getElementById("connectionStatus").textContent = "Disconnected";
            document.getElementById("connectionStatus").className = "disconnected";

            if (err.statusCode === 401) {
                addMessage("Authentication failed. Please login again.", 'error-message');
                logout();
            } else {
                setTimeout(startConnection, 5000);
            }
        }
    });
}

// Остальные функции без изменений...

// Обработчики событий
document.addEventListener("DOMContentLoaded", () => {
    // Обработчик логина
    document.getElementById("loginBtn").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
        event.preventDefault();
        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;

        if (!email || !password) {
            document.getElementById('loginStatus').textContent = 'Please enter email and password';
            document.getElementById('loginStatus').style.color = 'red';
            return;
        }

        yield login(email, password);
    }));

    // Обработчик логаута
    document.getElementById("logoutBtn").addEventListener("click", (event) => {
        event.preventDefault();
        logout();
    });

    // Инициализация приложения
    initializeApp();
});