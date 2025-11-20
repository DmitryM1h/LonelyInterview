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

// Инициализация SignalR соединения
function initializeConnection() {
    console.log("🔄 Initializing SignalR connection");

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/interview", {
            accessTokenFactory: () => {
                console.log("🔑 Providing auth token");
                return authToken;
            }
        })
        .configureLogging(signalR.LogLevel.Debug)
        .build();

    setupConnectionHandlers();
    startConnection();
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

// Функция логина
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

            // Инициализируем приложение
            showAppContent();
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
        if (!connection) return;

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

// Функция для инициализации аудио
function initializeAudio() {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            audioStream = yield navigator.mediaDevices.getUserMedia({
                audio: {
                    channelCount: 1,
                    sampleRate: 16000,
                    sampleSize: 16
                }
            });

            // Инициализация визуализации
            audioContext = new (window.AudioContext || window.webkitAudioContext)();
            analyser = audioContext.createAnalyser();
            const source = audioContext.createMediaStreamSource(audioStream);
            source.connect(analyser);
            analyser.fftSize = 256;
            dataArray = new Uint8Array(analyser.frequencyBinCount);

            // Инициализация MediaRecorder
            mediaRecorder = new MediaRecorder(audioStream, {
                mimeType: 'audio/webm;codecs=opus'
            });

            mediaRecorder.ondataavailable = (event) => {
                if (event.data.size > 0) {
                    audioChunks.push(event.data);
                    sendAudioChunk(event.data);
                }
            };

            mediaRecorder.onstop = () => {
                console.log("Recording stopped");
            };

            return true;
        } catch (error) {
            console.error("Error initializing audio:", error);
            addMessage(`Audio error: ${error.message}`, 'error-message');
            return false;
        }
    });
}

// Функция для отправки аудио чанка
function sendAudioChunk(chunk) {
    if (audioSubject && connection.state === signalR.HubConnectionState.Connected) {
        // Конвертируем Blob в base64 для передачи
        const reader = new FileReader();
        reader.onload = () => {
            const base64Data = reader.result.split(',')[1]; // Убираем data:audio/webm;base64,
            audioSubject.next(base64Data);
        };
        reader.readAsDataURL(chunk);
    }
}

// Функция визуализации аудио
function visualizeAudio() {
    if (!analyser || !isRecording) return;

    analyser.getByteFrequencyData(dataArray);

    canvasCtx.fillStyle = 'rgb(200, 200, 200)';
    canvasCtx.fillRect(0, 0, canvas.width, canvas.height);

    const barWidth = (canvas.width / dataArray.length) * 2.5;
    let barHeight;
    let x = 0;

    for (let i = 0; i < dataArray.length; i++) {
        barHeight = dataArray[i] / 2;

        canvasCtx.fillStyle = `rgb(${barHeight + 100}, 50, 50)`;
        canvasCtx.fillRect(x, canvas.height - barHeight, barWidth, barHeight);

        x += barWidth + 1;
    }

    requestAnimationFrame(visualizeAudio);
}

// Функция для добавления сообщений в список
function addMessage(text, className) {
    const li = document.createElement('li');
    li.textContent = text;
    li.className = className;
    document.getElementById('messagesList').appendChild(li);
}

// Обработчик кнопки Start Recording - ВЫЗЫВАЕТ МЕТОД ХАБА
document.getElementById("startRecording").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    if (connection.state !== signalR.HubConnectionState.Connected) {
        alert("Not connected to server");
        return;
    }

    try {
        const audioInitialized = yield initializeAudio();
        if (!audioInitialized) return;

        // Создаем Subject для потоковой передачи аудио
        audioSubject = new signalR.Subject();

        // ВЫЗОВ МЕТОДА ХАБА StartAudioStream
        yield connection.send("StartAudioStream", audioSubject);

        // Начинаем запись
        audioChunks = [];
        mediaRecorder.start(100); // Отправляем чанки каждые 100мс
        isRecording = true;

        // Обновляем UI
        document.getElementById("startRecording").disabled = true;
        document.getElementById("stopRecording").disabled = false;
        document.getElementById("startRecording").classList.add("recording");

        // Запускаем визуализацию
        visualizeAudio();

        addMessage("Audio recording started...", 'info-message');
        console.log("✅ Audio recording started and StartAudioStream method called");

    } catch (e) {
        console.error("❌ Error starting recording:", e);
        addMessage(`Recording error: ${e.toString()}`, 'error-message');
    }
    event.preventDefault();
}));

// Обработчик кнопки Stop Recording
document.getElementById("stopRecording").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    try {
        if (mediaRecorder && isRecording) {
            mediaRecorder.stop();
            isRecording = false;

            // Завершаем поток
            if (audioSubject) {
                audioSubject.complete();
                audioSubject = null;
            }

            // Останавливаем треки
            if (audioStream) {
                audioStream.getTracks().forEach(track => track.stop());
            }

            // Обновляем UI
            document.getElementById("startRecording").disabled = false;
            document.getElementById("stopRecording").disabled = true;
            document.getElementById("startRecording").classList.remove("recording");

            addMessage("Audio recording stopped", 'info-message');
            console.log("Audio recording stopped");
        }
    } catch (e) {
        console.error("Error stopping recording:", e);
        addMessage(`Stop recording error: ${e.toString()}`, 'error-message');
    }
    event.preventDefault();
}));

// Обработчик кнопки Upload (альтернативный способ вызова метода хаба)
document.getElementById("uploadButton").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    if (connection.state !== signalR.HubConnectionState.Connected) {
        alert("Not connected to server");
        return;
    }

    try {
        const subject = new signalR.Subject();
        connection.send("UploadStream", subject);

        addMessage("Starting upload...", 'info-message');

        let iteration = 0;
        const intervalHandle = setInterval(() => {
            iteration++;
            const data = `Upload item ${iteration} at ${new Date().toLocaleTimeString()}`;
            subject.next(data);

            addMessage(`Sent: ${data}`, 'client-message');

            if (iteration === 5) {
                clearInterval(intervalHandle);
                subject.complete();
                addMessage("Upload completed", 'info-message');
            }
        }, 1000);
    } catch (e) {
        console.error("Upload error:", e);
        addMessage(`Upload error: ${e.toString()}`, 'error-message');
    }
    event.preventDefault();
}));

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