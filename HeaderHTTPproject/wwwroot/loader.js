const form = document.getElementById('analyze-form');
const resultDiv = document.getElementById('result');

async function handleSubmit(event) {
    if (event) {
        event.preventDefault();
    }

    const formData = new FormData(form);

    resultDiv.innerHTML = 'Loading';

    let dots = 1;
    const interval = setInterval(() => {
        resultDiv.innerHTML = `Loading${'.'.repeat(dots)}`;
        dots = (dots + 1) % 4;
    }, 500);

    try {
        const response = await fetch(form.action, {
            method: form.method,
            body: formData
        });
        resultDiv.innerHTML = await response.text();
    } catch (e) {
        console.error(e);
        resultDiv.innerHTML = '<p>An error occurred while loading the result.</p>';
    }

    clearInterval(interval);
}

form.addEventListener('submit', handleSubmit);


form.addEventListener('submit', handleSubmit);

function handleScenarioChange() {
    const scenarioDropdown = document.getElementById('scenario');
    const urlInputSection = document.getElementById('urlInputSection');

    if (scenarioDropdown.value) {
        document.getElementById("result").innerHTML = "";
    }

    if (scenarioDropdown.value === 'yourUrl') {
        urlInputSection.style.display = 'grid';
    } else {
        urlInputSection.style.display = 'none';
        handleSubmit({ target: form, preventDefault: () => {} });
    }
}
