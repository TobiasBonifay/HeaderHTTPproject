const form = document.getElementById('analyze-form');
const resultDiv = document.getElementById('result');

form.addEventListener('submit', async event => {
    event.preventDefault();

    const formData = new FormData(event.target);

    resultDiv.innerHTML = 'Loading';

    let dots = 1;
    const interval = setInterval(() => {
        resultDiv.innerHTML = `Loading${'.'.repeat(dots)}`;
        dots = (dots + 1) % 4;
    }, 500);

    try {
        const response = await fetch(event.target.action, {
            method: event.target.method,
            body: formData
        });
        resultDiv.innerHTML = await response.text();
    } catch (e) {
        console.error(e);
        resultDiv.innerHTML = '<p>An error occurred while loading the result.</p>';
    }

    clearInterval(interval);
});