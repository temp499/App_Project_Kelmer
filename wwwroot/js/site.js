const appRequests = {
    token: () => document.querySelector('#request-token input').value,
    statusMessage: status => ({
        400: "La solicitud contiene datos inválidos.",
        401: "Inicie sesión para continuar.",
        403: "Su cuenta no tiene permiso para realizar esta acción.",
        404: "No se encontró el registro."
    })[status] ?? "No se pudo completar la operación.",
    async send(url, method = "GET", body) {
        const headers = { Accept: "application/json" };
        if (method !== "GET")
            headers.RequestVerificationToken = this.token();
        if (body && !(body instanceof FormData)) {
            headers["Content-Type"] = "application/json";
            body = JSON.stringify(body);
        }
        const response = await fetch(url, { method, headers, body });
        const content = response.headers.get("Content-Type") ?? "";
        const result = content.includes("application/json") ? await response.json() : {};
        if (!response.ok)
            throw new Error(result.message ?? result.errorMessage ?? this.statusMessage(response.status));
        return result;
    }
};

function showActionMessage(message, error = false) {
    const alert = document.querySelector("#action-message");
    alert.textContent = message;
    alert.className = `alert alert-${error ? "danger" : "success"}`;
}

for (const confirmation of document.querySelectorAll("[data-confirm-password]")) {
    const form = confirmation.form;
    const password = form.elements.namedItem(confirmation.dataset.confirmPassword);
    const validate = () => confirmation.setCustomValidity(
        confirmation.value && confirmation.value !== password.value ? "Las contraseñas no coinciden." : "");

    password.addEventListener("input", validate);
    confirmation.addEventListener("input", validate);
    form.addEventListener("submit", event => {
        validate();
        if (!confirmation.reportValidity()) event.preventDefault();
    });
}

document.addEventListener("submit", async event => {
    const form = event.target;
    if (event.defaultPrevented || !form.matches("form[data-ajax]")) return;
    event.preventDefault();
    if (form.dataset.confirm && !confirm(form.dataset.confirm)) return;

    const button = event.submitter;
    if (button) button.disabled = true;
    try {
        const fields = new FormData(form);
        let body = fields;
        if (form.dataset.ajax === "json") {
            fields.delete("__RequestVerificationToken");
            body = Object.fromEntries(fields);
            for (const input of form.querySelectorAll("[data-number]"))
                body[input.name] = Number(input.value);
            for (const input of form.querySelectorAll("[data-boolean]"))
                body[input.name] = input.value === "true";
        }
        const result = await appRequests.send(form.action, "POST", body);
        if (form.dataset.redirect) {
            if (result.message) sessionStorage.setItem("app-message", result.message);
            location.assign(form.dataset.redirect);
        } else if (form.hasAttribute("data-reload")) {
            sessionStorage.setItem("app-message", result.message ?? "Cambios guardados.");
            location.reload();
        } else {
            showActionMessage(result.message ?? "Cambios guardados.");
            if (form.hasAttribute("data-reset")) form.reset();
        }
    } catch (error) {
        showActionMessage(error.message, true);
    } finally {
        if (button) button.disabled = false;
    }
});

const savedActionMessage = sessionStorage.getItem("app-message");
if (savedActionMessage) {
    sessionStorage.removeItem("app-message");
    showActionMessage(savedActionMessage);
}

document.querySelector("[data-resend]")?.addEventListener("click", async event => {
    const button = event.currentTarget;
    const email = document.querySelector("#Email");
    if (!email.reportValidity()) return;
    button.disabled = true;
    try {
        const result = await appRequests.send(button.dataset.resend, "POST", { email: email.value });
        showActionMessage(result.message);
    } catch (error) {
        showActionMessage(error.message, true);
    } finally {
        button.disabled = false;
    }
});
