import axios from 'axios'; // uvoxi axiod biblioteku koja zna kako da salje http zahteve

const api = axios.create({
    baseURL: '/api',
    headers: {'Content-Type': 'application/json' } //podesavamo bazu za url i tip podataka(JSON) koji se salje backendu
});

// dodajemo JWT token za svaki zahtev:
api.interceptors.request.use(config => {
    const raw = localStorage.getItem('auth');
    if(raw) {
        const parsed = JSON.parse(raw);
        const token = parsed?.state?.accessToken;
        if (token) {
            config.headers.Autorization = `Bearer ${token}`;
        }
    }
    return config;
});

export default api;