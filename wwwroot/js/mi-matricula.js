const enrollmentForm = document.querySelector("#enrollment-form");
const enrollmentFields = document.querySelector("#enrollment-fields");
const periodSelect = document.querySelector("#periodo");
const offerBody = document.querySelector("#available-groups");
const offerStatus = document.querySelector("#offer-status");
const enrollmentButton = document.querySelector("#enroll-button");
let ownHistory = [];
let loadingOffer = false;

function enrollmentCell(row, text) {
    const cell = document.createElement("td");
    cell.textContent = text;
    row.append(cell);
    return cell;
}

function enrollmentSchedule(horarios) {
    const days = ["", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"];
    return horarios.map(item => `${days[item.diaSemana]} ${item.horaInicio.slice(0, 5)} - ${item.horaFin.slice(0, 5)} (${item.aula})`).join("; ");
}

function selectedGroups() {
    return [...offerBody.querySelectorAll("input:checked")].map(input => Number(input.value));
}

function updateEnrollmentSelection() {
    const count = selectedGroups().length;
    document.querySelector("#selected-count").textContent = `${count} grupos seleccionados`;
    enrollmentButton.disabled = loadingOffer || count === 0;
}

async function loadEnrollmentHistory() {
    ownHistory = await appRequests.send("/MiMatricula/Historial");
    const body = document.querySelector("#enrollment-history");
    body.replaceChildren();
    for (const item of ownHistory) {
        const row = document.createElement("tr");
        enrollmentCell(row, item.id);
        enrollmentCell(row, item.periodoNombre);
        enrollmentCell(row, item.cursos.map(curso => `${curso.nombre}${curso.grupo ? " - " + curso.grupo : ""}`).join("; "));
        enrollmentCell(row, item.cursos.map(curso => enrollmentSchedule(curso.horarios)).filter(Boolean).join("; ") || "Sin horario registrado");
        body.append(row);
    }
    document.querySelector("#history-status").textContent = ownHistory.length
        ? `${ownHistory.length} matriculas registradas` : "Todavia no tienes matriculas registradas.";
}

async function loadEnrollmentOffer() {
    offerBody.replaceChildren();
    loadingOffer = true;
    periodSelect.disabled = true;
    updateEnrollmentSelection();
    try {
        if (!periodSelect.value) {
            offerStatus.textContent = "Selecciona un periodo para consultar los grupos.";
            return;
        }
        if (ownHistory.some(item => item.periodoAcademicoId === Number(periodSelect.value))) {
            offerStatus.textContent = "Ya tienes una matricula en este periodo. Puedes consultarla en tu historial.";
            return;
        }
        offerStatus.textContent = "Cargando grupos...";
        const groups = await appRequests.send(`/MiMatricula/Oferta?periodoId=${periodSelect.value}`);
        for (const group of groups) {
            const row = document.createElement("tr");
            const cell = enrollmentCell(row, "");
            const input = document.createElement("input");
            input.type = "checkbox";
            input.className = "form-check-input";
            input.value = group.id;
            input.dataset.course = group.cursoId;
            input.disabled = group.disponibles <= 0 || group.horarios.length === 0;
            input.setAttribute("aria-label", `Elegir ${group.cursoNombre}, grupo ${group.grupo}`);
            const label = document.createElement("label");
            label.className = "group-choice";
            label.append(input);
            cell.append(label);
            enrollmentCell(row, `${group.cursoNombre} - ${group.grupo}`);
            enrollmentCell(row, `${group.docente} - ${group.sede}`);
            enrollmentCell(row, enrollmentSchedule(group.horarios) || "Sin horario disponible");
            enrollmentCell(row, `${group.disponibles} de ${group.cupo}`);
            offerBody.append(row);
        }
        offerStatus.textContent = groups.length
            ? "Elige un grupo por curso. Los cupos se validaran al confirmar."
            : "No hay grupos disponibles para tu carrera en este periodo.";
    } catch (error) {
        offerStatus.textContent = "No se pudo cargar la oferta. Vuelve a seleccionar el periodo para reintentar.";
        showActionMessage(error.message, true);
    } finally {
        loadingOffer = false;
        periodSelect.disabled = false;
        updateEnrollmentSelection();
    }
}
offerBody.addEventListener("change", event => {
    if (event.target.checked) {
        for (const input of offerBody.querySelectorAll("input:checked")) {
            if (input !== event.target && input.dataset.course === event.target.dataset.course) {
                input.checked = false;
            }
        }
    }
    updateEnrollmentSelection();
});
periodSelect.addEventListener("change", loadEnrollmentOffer);
enrollmentForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    const groups = selectedGroups();
    if (groups.length === 0 || loadingOffer) {
        return;
    }
    if (!confirm(`Confirmar la matricula de ${groups.length} grupos?`)) {
        return;
    }
    enrollmentFields.disabled = true;
    enrollmentButton.textContent = "Guardando...";
    try {
        const result = await appRequests.send("/MiMatricula/Create", "POST", {
            periodoAcademicoId: Number(periodSelect.value), cursoIds: [], grupoIds: groups
        });
        showActionMessage(result.message);
        await loadEnrollmentHistory();
        await loadEnrollmentOffer();
    } catch (error) {
        showActionMessage(error.message, true);
        await loadEnrollmentOffer();
    } finally {
        enrollmentFields.disabled = false;
        enrollmentButton.textContent = "Confirmar matricula";
        updateEnrollmentSelection();
    }
});

async function initializeEnrollment() {
    try {
        const profile = await appRequests.send("/MiMatricula/Perfil");
        document.querySelector("#student-profile").textContent = `${profile.nombre} ${profile.apellido} - ${profile.carreraNombre || "Sin carrera asignada"}`;
        await loadEnrollmentHistory();
        if (!profile.carreraId) {
            offerStatus.textContent = "Solicita al administrador que asigne tu carrera antes de matricular.";
            return;
        }
        const periods = await appRequests.send("/MiMatricula/Periodos");
        for (const period of periods) {
            const option = document.createElement("option");
            option.value = period.id;
            option.textContent = period.nombre;
            periodSelect.append(option);
        }
        enrollmentFields.disabled = periods.length === 0;
        if (periods.length === 0) {
            offerStatus.textContent = "No hay periodos de matricula abiertos.";
        }
    } catch (error) {
        document.querySelector("#student-profile").textContent = "No se pudieron cargar tus datos. Si tu cuenta aun no esta vinculada, solicita la vinculacion al administrador.";
        document.querySelector("#history-status").textContent = "No se pudo cargar el historial.";
        showActionMessage(error.message, true);
    }
}
initializeEnrollment();
