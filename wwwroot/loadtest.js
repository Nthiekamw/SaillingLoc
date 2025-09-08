import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '2m', target: 1000 },   // montée progressive à 1000 utilisateurs
        { duration: '5m', target: 50000 },  // montée jusqu'à 50 000
        { duration: '2m', target: 0 },      // descente
    ],
};

export default function () {
    let url = 'https://tonapp.com/Login';
    let payload = JSON.stringify({
        username: 'user1',
        password: 'password123'
    });

    let params = {
        headers: { 'Content-Type': 'application/json' },
    };

    let res = http.post(url, payload, params);
    check(res, { 'status is 200': (r) => r.status === 200 });
    sleep(1);
}
                                                                                                