const academicModules = {
    Facultad: {
        title: "Facultades",
        fields: [
            ["nombre", "Nombre", "text"],
            ["descripcion", "Descripcion", "textarea"]
        ]
    },
    Carrera: {
        title: "Carreras",
        fields: [
            ["nombre", "Nombre", "text"],
            ["facultadId", "Facultad", "Facultad"]
        ]
    },
    Sede: {
        title: "Sedes",
        fields: [
            ["nombre", "Nombre", "text"],
            ["descripcion", "Descripcion", "textarea"]
        ]
    },
    Aula: {
        title: "Aulas",
        fields: [
            ["nombre", "Nombre", "text"],
            ["sedeId", "Sede", "Sede"],
            ["capacidad", "Capacidad", "number"]
        ]
    },
    Docente: {
        title: "Docentes",
        fields: [
            ["nombre", "Nombre", "text"],
            ["apellido", "Apellido", "text"],
            ["email", "Correo", "email"]
        ]
    },
    PeriodoAcademico: {
        title: "Periodos",
        noDelete: true,
        fields: [
            ["nombre", "Nombre", "text"],
            ["matriculaAbierta", "Matricula abierta", "boolean"]
        ]
    },
    PlanEstudio: {
        title: "Planes de estudio",
        fields: [
            ["nombre", "Nombre", "text"],
            ["carreraId", "Carrera", "Carrera"],
            ["activo", "Activo", "boolean"]
        ]
    },
    DetallePlanEstudio: {
        title: "Cursos del plan",
        fields: [
            ["planEstudioId", "Plan", "PlanEstudio"],
            ["cursoId", "Curso", "Cursos"],
            ["creditos", "Creditos", "number"],
            ["semestre", "Semestre", "number"]
        ]
    },
    OfertaAcademica: {
        title: "Oferta academica",
        fields: [
            ["nombre", "Nombre", "text"],
            ["planEstudioId", "Plan", "PlanEstudio"],
            ["periodoAcademicoId", "Periodo", "PeriodoAcademico"],
            ["sedeId", "Sede", "Sede"],
            ["activa", "Activa", "boolean"]
        ]
    },
    DetalleOfertaAcademica: {
        title: "Grupos",
        fields: [
            ["grupo", "Grupo", "text"],
            ["ofertaAcademicaId", "Oferta", "OfertaAcademica"],
            ["cursoId", "Curso", "Cursos"],
            ["docenteId", "Docente", "Docente"],
            ["cupo", "Cupo", "number"]
        ]
    },
    DetalleHorario: {
        title: "Horarios",
        fields: [
            ["detalleOfertaAcademicaId", "Grupo", "DetalleOfertaAcademica"],
            ["aulaId", "Aula", "Aula"],
            ["diaSemana", "Dia", "day"],
            ["horaInicio", "Hora de inicio", "time"],
            ["horaFin", "Hora de fin", "time"]
        ]
    }
};
const academicSelect = document.querySelector("#academic-module");
const academicForm = document.querySelector("#academic-form");
const academicModal = new bootstrap.Modal("#academic-modal");
const academicReferences = {};
const academicTypes = ["text", "textarea", "email", "number", "boolean", "day", "time"];
let academicRows = [];
let academicLoading = false;

function academicName(source, item) {
    if (source === "DetalleOfertaAcademica") {
        const course = academicReferences.Cursos?.find(record => record.id === item.cursoId);
        const offer = academicReferences.OfertaAcademica?.find(record => record.id === item.ofertaAcademicaId);
        return `${course?.nombre || "Curso"} - ${item.grupo} / ${offer?.nombre || "Oferta"}`;
    }
    return `${item.nombre || ""}${item.apellido ? " " + item.apellido : ""}`;
}

function academicText(field, value) {
    const type = field[2];
    if (type === "boolean") return value ? "Si" : "No";
    if (type === "day") return ["", "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"][value];
    if (type === "time") return value?.slice(0, 5) || "";
    if (academicReferences[type]) {
        const item = academicReferences[type].find(record => record.id === value);
        return item ? academicName(type, item) : "Sin referencia";
    }
    return value ?? "";
}

function renderAcademicRows() {
    const module = academicModules[academicSelect.value];
    const body = document.querySelector("#academic-body");
    const search = document.querySelector("#academic-search").value.toLowerCase().trim();
    body.replaceChildren();
    for (const item of academicRows) {
        const values = module.fields.map(field => academicText(field, item[field[0]]));
        if (search && !values.join(" ").toLowerCase().includes(search)) continue;
        const row = document.createElement("tr");
        for (const value of values) {
            const cell = document.createElement("td");
            cell.textContent = value;
            row.append(cell);
        }
        const actions = document.createElement("td");
        actions.className = "text-nowrap";
        for (const action of module.noDelete ? ["Editar"] : ["Editar", "Eliminar"]) {
            const button = document.createElement("button");
            button.type = "button";
            button.className = `btn btn-sm me-2 ${action === "Editar" ? "btn-outline-primary" : "btn-outline-danger"}`;
            button.textContent = action;
            button.addEventListener("click", () => action === "Editar" ? openAcademicEditor(item) : deleteAcademicItem(item, button));
            actions.append(button);
        }
        row.append(actions);
        body.append(row);
    }
    document.querySelector("#academic-status").textContent = body.rows.length ? `${body.rows.length} registros` : "No hay registros para mostrar.";
}

async function loadAcademicModule() {
    academicLoading = true;
    academicSelect.disabled = true;
    document.querySelector("#academic-new").disabled = true;
    document.querySelector("#academic-body").replaceChildren();
    document.querySelector("#academic-status").textContent = "Cargando registros...";
    try {
        const module = academicModules[academicSelect.value];
        document.querySelector("#academic-title").textContent = module.title;
        const sources = new Set(module.fields.map(field => field[2]).filter(type => !academicTypes.includes(type)));
        if (sources.has("DetalleOfertaAcademica")) {
            sources.add("Cursos");
            sources.add("OfertaAcademica");
        }
        await Promise.all([...sources].map(async source => {
            academicReferences[source] = await appRequests.send(source === "Cursos" ? "/MiMatricula/Cursos" : `/${source}/GetData`);
        }));
        academicRows = await appRequests.send(`/${academicSelect.value}/GetData`);
        const header = document.createElement("tr");
        for (const text of [...module.fields.map(field => field[1]), "Acciones"]) {
            const cell = document.createElement("th");
            cell.scope = "col";
            cell.textContent = text;
            header.append(cell);
        }
        document.querySelector("#academic-head").replaceChildren(header);
        renderAcademicRows();
        document.querySelector("#academic-new").disabled = false;
    } catch (error) {
        academicRows = [];
        document.querySelector("#academic-status").textContent = "No se pudo cargar el apartado. Seleccionalo de nuevo para reintentar.";
        showActionMessage(error.message, true);
    } finally {
        academicLoading = false;
        academicSelect.disabled = false;
    }
}

function addAcademicOption(select, value, text) {
    const option = document.createElement("option");
    option.value = value;
    option.textContent = text;
    select.append(option);
}

function openAcademicEditor(item) {
    const fields = document.querySelector("#academic-fields");
    fields.replaceChildren();
    academicForm.reset();
    academicForm.elements.id.value = item?.id || "";
    document.querySelector("#academic-error").classList.add("d-none");
    document.querySelector("#academic-editor-title").textContent = item ? "Editar registro" : "Nuevo registro";
    for (const [name, label, type] of academicModules[academicSelect.value].fields) {
        const container = document.createElement("div");
        container.className = "mb-3";
        const caption = document.createElement("label");
        caption.className = "form-label";
        caption.htmlFor = `academic-${name}`;
        caption.textContent = label;
        const reference = academicReferences[type];
        const select = reference || type === "boolean" || type === "day";
        const input = document.createElement(select ? "select" : type === "textarea" ? "textarea" : "input");
        input.className = select ? "form-select" : "form-control";
        input.id = caption.htmlFor;
        input.name = name;
        input.required = type !== "textarea";
        if (reference) {
            addAcademicOption(input, "", "Selecciona una opcion");
            for (const option of reference) addAcademicOption(input, option.id, academicName(type, option));
        } else if (type === "boolean") {
            addAcademicOption(input, "false", "No");
            addAcademicOption(input, "true", "Si");
        } else if (type === "day") {
            addAcademicOption(input, "", "Selecciona un dia");
            ["Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"].forEach((day, index) => addAcademicOption(input, index + 1, day));
        } else {
            input.type = type;
            if (type === "number") { input.min = "1"; input.max = "2147483647"; input.step = "1"; }
            if (type === "text") { input.minLength = name === "grupo" ? 1 : 2; input.maxLength = name === "grupo" ? 20 : 100; }
            if (type === "email") input.maxLength = 150;
            if (type === "textarea") input.maxLength = 250;
        }
        if (item) input.value = type === "time" ? item[name].slice(0, 5) : String(item[name] ?? "");
        container.append(caption, input);
        fields.append(container);
    }
    academicModal.show();
}

async function deleteAcademicItem(item, button) {
    if (!confirm("Eliminar este registro? No se podra eliminar si esta relacionado con otros datos.")) return;
    button.disabled = true;
    try {
        const result = await appRequests.send(`/${academicSelect.value}/Delete?id=${item.id}`, "POST");
        showActionMessage(result.message);
        await loadAcademicModule();
    } catch (error) { showActionMessage(error.message, true); }
    finally { button.disabled = false; }
}

academicForm.addEventListener("submit", async event => {
    event.preventDefault();
    const button = document.querySelector("#academic-save");
    button.disabled = true;
    const data = { id: Number(academicForm.elements.id.value) || null };
    for (const [name, label, type] of academicModules[academicSelect.value].fields) {
        const value = academicForm.elements.namedItem(name).value;
        data[name] = type === "boolean" ? value === "true" : type === "time" ? value + ":00"
            : type === "number" || type === "day" || academicReferences[type] ? Number(value) : value;
    }
    try {
        const result = await appRequests.send(`/${academicSelect.value}/${data.id ? "Update" : "Create"}`, "POST", data);
        academicModal.hide();
        showActionMessage(result.message);
        await loadAcademicModule();
    } catch (error) {
        const alert = document.querySelector("#academic-error");
        alert.textContent = error.message;
        alert.classList.remove("d-none");
    } finally { button.disabled = false; }
});

document.querySelector("#academic-new").addEventListener("click", () => openAcademicEditor());
document.querySelector("#academic-search").addEventListener("input", () => { if (!academicLoading) renderAcademicRows(); });
academicSelect.addEventListener("change", () => {
    document.querySelector("#academic-search").value = "";
    loadAcademicModule();
});
loadAcademicModule();
