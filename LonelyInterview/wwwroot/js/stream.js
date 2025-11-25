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
        // НЕ обновляем статус сразу, проверяем действительно ли отключено
        setTimeout(() => {
            if (connection.state === signalR.HubConnectionState.Disconnected) {
                updateConnectionStatus("Disconnected", "disconnected");
            }
        }, 1000);
    });

    connection.onreconnecting((error) => {
        console.log("Reconnecting: ", error);
        updateConnectionStatus("Reconnecting...", "reconnecting");
    });

    connection.onreconnected((connectionId) => {
        console.log("Reconnected: ", connectionId);
        updateConnectionStatus("Connected", "connected");
    });

    // ДОБАВИМ обработчик для ошибок вызовов методов
    connection.on("ReceiveError", (error) => {
        console.error("Server error:", error);
        addChatMessage(`Server error: ${error}`, 'error-message');
    });

    // НОВЫЕ ОБРАБОТЧИКИ ДЛЯ ЧАТА
    connection.on("AudioProcessingDelay", (data) => {
        addChatMessage(`⚠️ ${data.Message}`, 'system-message');
    });

    connection.on("CodeSubmitted", (data) => {
        addChatMessage(`✅ ${data.Message}`, 'system-message');
    });

    connection.on("CodeError", (data) => {
        addChatMessage(`❌ Ошибка: ${data.Error}`, 'error-message');
    });

    // ДОБАВИМ обработчик успешного начала аудиопотока
    connection.on("AudioStreamStarted", (data) => {
        console.log("✅ Audio stream started successfully:", data);
        addChatMessage("Аудиопоток начал передаваться", 'system-message');
    });
}

// НОВАЯ ФУНКЦИЯ для безопасного обновления статуса
function updateConnectionStatus(text, className) {
    const statusElement = document.getElementById("connectionStatus");
    if (statusElement) {
        statusElement.textContent = text;
        statusElement.className = className;
    }
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
    addChatMessage("Logged out successfully", 'info-message');
}

function startConnection() {
    return __awaiter(this, void 0, void 0, function* () {
        if (!connection) return;

        try {
            yield connection.start();
            console.log("✅ Connected successfully to SignalR");
            updateConnectionStatus("Connected", "connected");

            // ДОБАВИМ приветственное сообщение после успешного соединения
            addChatMessage("Добро пожаловать! Введите ваш код в поле ниже и нажмите 'Отправить код' или Ctrl+Enter", 'system-message');

        } catch (err) {
            console.error("❌ Connection failed: ", err);
            updateConnectionStatus("Disconnected", "disconnected");

            if (err.statusCode === 401) {
                addChatMessage("Authentication failed. Please login again.", 'error-message');
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
            addChatMessage(`Audio error: ${error.message}`, 'error-message');
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

// Функции для чата кода
function addChatMessage(message, type = 'user-message') {
    const chatMessages = document.getElementById('chatMessages');
    if (!chatMessages) return;

    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type}`;

    const timestamp = document.createElement('div');
    timestamp.className = 'timestamp';
    timestamp.textContent = new Date().toLocaleTimeString();

    const content = document.createElement('div');
    content.textContent = message;

    messageDiv.appendChild(timestamp);
    messageDiv.appendChild(content);
    chatMessages.appendChild(messageDiv);

    // Автопрокрутка вниз
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

function addCodeMessage(code, type = 'code-message') {
    const chatMessages = document.getElementById('chatMessages');
    if (!chatMessages) return;

    const messageDiv = document.createElement('div');
    messageDiv.className = `message ${type}`;

    const timestamp = document.createElement('div');
    timestamp.className = 'timestamp';
    timestamp.textContent = new Date().toLocaleTimeString();

    const content = document.createElement('div');
    content.textContent = code;
    content.style.whiteSpace = 'pre-wrap';
    content.style.fontFamily = 'Courier New, monospace';

    messageDiv.appendChild(timestamp);
    messageDiv.appendChild(content);
    chatMessages.appendChild(messageDiv);

    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// ОБНОВЛЕННЫЙ обработчик Start Recording
document.getElementById("startRecording").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    // ПРОВЕРЯЕМ реальное состояние соединения, а не только UI статус
    if (connection.state !== signalR.HubConnectionState.Connected) {
        const actualState = connection.state;
        console.warn(`Cannot start recording - connection state: ${actualState}`);
        addChatMessage(`Не могу начать запись. Статус соединения: ${actualState}`, 'error-message');
        return;
    }

    try {
        const audioInitialized = yield initializeAudio();
        if (!audioInitialized) return;

        // Создаем Subject для потоковой передачи аудио
        audioSubject = new signalR.Subject();

        console.log("🔄 Calling StartAudioStream method...");

        // ВЫЗОВ МЕТОДА ХАБА StartAudioStream с обработкой ошибок
        yield connection.send("StartAudioStream", audioSubject)
            .then(() => {
                console.log("✅ StartAudioStream method called successfully");
            })
            .catch(error => {
                console.error("❌ StartAudioStream method failed:", error);
                throw error;
            });

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

        addChatMessage("Аудиозапись начата...", 'info-message');
        console.log("✅ Audio recording started and StartAudioStream method called");

    } catch (e) {
        console.error("❌ Error starting recording:", e);
        addChatMessage(`Ошибка записи: ${e.toString()}`, 'error-message');

        // Восстанавливаем UI при ошибке
        document.getElementById("startRecording").disabled = false;
        document.getElementById("stopRecording").disabled = true;
        document.getElementById("startRecording").classList.remove("recording");
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

            addChatMessage("Audio recording stopped", 'info-message');
            console.log("Audio recording stopped");
        }
    } catch (e) {
        console.error("Error stopping recording:", e);
        addChatMessage(`Stop recording error: ${e.toString()}`, 'error-message');
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

        addChatMessage("Starting upload...", 'info-message');

        let iteration = 0;
        const intervalHandle = setInterval(() => {
            iteration++;
            const data = `Upload item ${iteration} at ${new Date().toLocaleTimeString()}`;
            subject.next(data);

            addChatMessage(`Sent: ${data}`, 'client-message');

            if (iteration === 5) {
                clearInterval(intervalHandle);
                subject.complete();
                addChatMessage("Upload completed", 'info-message');
            }
        }, 1000);
    } catch (e) {
        console.error("Upload error:", e);
        addChatMessage(`Upload error: ${e.toString()}`, 'error-message');
    }
    event.preventDefault();
}));

// Обработчик отправки кода
document.getElementById("sendCodeBtn").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    event.preventDefault();

    const codeInput = document.getElementById("codeInput");
    const code = codeInput.value.trim();

    if (!code) {
        addChatMessage("Пожалуйста, введите код", 'system-message');
        return;
    }

    if (connection.state !== signalR.HubConnectionState.Connected) {
        addChatMessage("Нет соединения с сервером", 'error-message');
        return;
    }

    try {
        // Добавляем код в чат
        addCodeMessage(code, 'user-message');
        addChatMessage("Отправка кода на сервер...", 'system-message');

        // ВЫЗОВ МЕТОДА ХАБА SubmitCode
        yield connection.invoke("SubmitCode", code);

        addChatMessage("Код успешно отправлен!", 'system-message');

        // Очищаем поле ввода
        codeInput.value = "";

    } catch (e) {
        console.error("❌ Error submitting code:", e);
        addChatMessage(`Ошибка отправки кода: ${e.toString()}`, 'error-message');
    }
}));

// Обработчик очистки кода
document.getElementById("clearCodeBtn").addEventListener("click", (event) => {
    event.preventDefault();
    document.getElementById("codeInput").value = "";
    addChatMessage("Поле ввода очищено", 'system-message');
});

// Обработчик клавиши Enter в поле кода (Ctrl+Enter для отправки)
document.getElementById("codeInput").addEventListener("keydown", (event) => {
    if (event.ctrlKey && event.key === 'Enter') {
        event.preventDefault();
        document.getElementById("sendCodeBtn").click();
    }
});

// ДОБАВИМ функцию для периодической проверки реального статуса соединения
function startConnectionMonitor() {
    setInterval(() => {
        if (connection) {
            const actualState = connection.state;
            const displayedStatus = document.getElementById("connectionStatus").textContent;

            // Если UI статус не соответствует реальному состоянию - исправляем
            if ((actualState === signalR.HubConnectionState.Connected && displayedStatus !== "Connected") ||
                (actualState === signalR.HubConnectionState.Connecting && displayedStatus !== "Reconnecting...") ||
                (actualState === signalR.HubConnectionState.Disconnected && displayedStatus !== "Disconnected")) {

                console.log(`🔄 Correcting UI status: ${displayedStatus} -> ${actualState}`);

                switch (actualState) {
                    case signalR.HubConnectionState.Connected:
                        updateConnectionStatus("Connected", "connected");
                        break;
                    case signalR.HubConnectionState.Connecting:
                    case signalR.HubConnectionState.Reconnecting:
                        updateConnectionStatus("Reconnecting...", "reconnecting");
                        break;
                    case signalR.HubConnectionState.Disconnected:
                        updateConnectionStatus("Disconnected", "disconnected");
                        break;
                }
            }
        }
    }, 2000); // Проверяем каждые 2 секунды
}

// ДОБАВИМ логирование для отладки
function logConnectionState() {
    if (connection) {
        console.log(`🔌 Current connection state: ${connection.state}`);
        console.log(`📊 Connection ID: ${connection.connectionId}`);
    }
}

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

        // ЗАПУСКАЕМ мониторинг соединения после логина
        if (connection) {
            setTimeout(startConnectionMonitor, 1000);
        }
    }));

    // Обработчик логаута
    document.getElementById("logoutBtn").addEventListener("click", (event) => {
        event.preventDefault();
        logout();
    });

    // Инициализация приложения
    initializeApp();
});

// Обработчик для тестовой кнопки
document.getElementById("testReceiveAnswers").addEventListener("click", (event) => __awaiter(this, void 0, void 0, function* () {
    event.preventDefault();

    if (connection.state !== signalR.HubConnectionState.Connected) {
        addChatMessage("Нет соединения с сервером", 'error-message');
        return;
    }

    try {
        console.log("🔄 Calling ReceiveModelAnswers method...");
        addChatMessage("Вызываю ReceiveModelAnswers...", 'system-message');

        // ВЫЗОВ МЕТОДА ХАБА ReceiveModelAnswers
        yield connection.invoke("ReceiveModelAnswers")
            .then(() => {
                console.log("✅ ReceiveModelAnswers method called successfully");
                addChatMessage("ReceiveModelAnswers вызван успешно!", 'system-message');
            })
            .catch(error => {
                console.error("❌ ReceiveModelAnswers method failed:", error);
                addChatMessage(`Ошибка: ${error.toString()}`, 'error-message');
            });

    } catch (e) {
        console.error("❌ Error calling ReceiveModelAnswers:", e);
        addChatMessage(`Ошибка вызова: ${e.toString()}`, 'error-message');
    }
}));