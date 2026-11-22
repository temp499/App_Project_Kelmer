const adminStudent = document.querySelector("#admin-student");
const studentAccount = document.querySelector("#student-account");
const studentCareer = document.querySelector("#student-career");
const previousHistory = document.querySelector("#history-previous");
const nextHistory = document.querySelector("#history-next");
let historyPage = 1;
let historyLoading = false;

async function showEnrollmentDetail(id, button) {
    const studentId = adminStudent.value;
    button.disabled = true;
    try {
        const item = await appRequests.send(`/Matricula/GetById?id=${id}`);
        if (adminStudent.value !== studentId) return;
        const list = document.querySelector("#detail-courses");
        list.replaceChildren();
        document.querySelector("#detail-title").textContent = `${item.periodoNombre} - Matricula ${item.id}`;
        const days = ["", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"];
        for (const course of item.cursos) {
            const row = document.createElement("li");
            row.className = "list-group-item";
            const schedule = course.horarios.map(horario => `${days[horario.diaSemana]} ${horario.horaInicio.slice(0, 5)} - ${horario.horaFin.slice(0, 5)}, ${horario.aula}`).join("; ");
            row.textContent = `${course.nombre}${course.grupo ? " - " + course.grupo : ""}${schedule ? ": " + schedule : ""}`;
            list.append(row);
        }
        document.querySelector("#enrollment-detail").classList.remove("d-none");
        document.querySelector("#enrollment-detail").scrollIntoView({ block: "nearest" });
    } catch (error) { showActionMessage(error.message, true); }
    finally { button.disabled = false; }
}

async function loadAdminHistory() {
    const body = document.querySelector("#admin-history");
    body.replaceChildren();
    previousHistory.disabled = true;
    nextHistory.disabled = true;
    document.querySelector("#enrollment-detail").classList.add("d-none");
    document.querySelector("#history-page").textContent = `Pagina ${historyPage}`;
    if (!adminStudent.value) {
        document.querySelector("#admin-history-status").textContent = "Selecciona un estudiante para consultar sus matriculas.";
        return;
    }
    historyLoading = true;
    adminStudent.disabled = true;
    document.querySelector("#student-settings").disabled = true;
    document.querySelector("#admin-history-status").textContent = "Cargando matriculas...";
    try {
        const history = await appRequests.send(`/Matricula/GetHistorial?EstudianteId=${adminStudent.value}&Pagina=${historyPage}`);
        for (const item of history.matriculas) {
            const row = document.createElement("tr");
            for (const text of [item.id, item.periodoNombre, item.totalCursos]) {
                const cell = document.createElement("td");
                cell.textContent = text;
                row.append(cell);
            }
            const cell = document.createElement("td");
            const button = document.createElement("button");
            button.type = "button";
            button.className = "btn btn-sm btn-outline-primary";
            button.textContent = "Ver cursos";
            button.addEventListener("click", () => showEnrollmentDetail(item.id, button));
            cell.append(button);
            row.append(cell);
            body.append(row);
        }
        document.querySelector("#admin-history-status").textContent = history.totalRegistros
            ? `${history.totalRegistros} matriculas registradas` : "Este estudiante no tiene matriculas registradas.";
        previousHistory.disabled = historyPage <= 1;
        nextHistory.disabled = historyPage * 10 >= history.totalRegistros;
    } catch (error) {
        document.querySelector("#admin-history-status").textContent = "No se pudo cargar el historial. Selecciona el estudiante para reintentar.";
        showActionMessage(error.message, true);
    } finally {
        historyLoading = false;
        adminStudent.disabled = false;
        updateStudentSettings();
    }
}

function updateStudentSettings() {
    const option = adminStudent.selectedOptions[0];
    document.querySelector("#student-settings").disabled = !adminStudent.value;
    studentAccount.value = option?.dataset.account || "";
    studentCareer.value = option?.dataset.career || "";
    const linked = Boolean(option?.dataset.account);
    studentAccount.disabled = linked;
    document.querySelector("#link-button").disabled = linked;
    document.querySelector("#account-status").textContent = linked
        ? "Este estudiante ya tiene una cuenta vinculada." : "Selecciona la cuenta que pertenece al estudiante.";
}

adminStudent.addEventListener("change", () => {
    historyPage = 1;
    updateStudentSettings();
    loadAdminHistory();
});
previousHistory.addEventListener("click", () => {
    if (historyLoading || historyPage <= 1) return;
    historyPage--;
    loadAdminHistory();
});
nextHistory.addEventListener("click", () => {
    if (historyLoading) return;
    historyPage++;
    loadAdminHistory();
});

async function saveStudentSetting(event, action, name, select) {
    event.preventDefault();
    if (!adminStudent.value || !select.value) return;
    if (!confirm("Guardar esta asignacion para el estudiante seleccionado?")) return;
    const button = event.submitter;
    const option = adminStudent.selectedOptions[0];
    const studentId = adminStudent.value;
    const value = select.value;
    button.disabled = true;
    adminStudent.disabled = true;
    document.querySelector("#student-settings").disabled = true;
    try {
        const result = await appRequests.send(`/Estudiante/${action}?id=${studentId}`, "POST", { [name]: Number(value) });
        showActionMessage(result.message);
        option.dataset[name === "usuarioId" ? "account" : "career"] = value;
    } catch (error) { showActionMessage(error.message, true); }
    finally {
        button.disabled = false;
        adminStudent.disabled = false;
        updateStudentSettings();
    }
}

document.querySelector("#link-account").addEventListener("submit", event => saveStudentSetting(event, "VincularUsuario", "usuarioId", studentAccount));
document.querySelector("#assign-career").addEventListener("submit", event => saveStudentSetting(event, "AsignarCarrera", "carreraId", studentCareer));
