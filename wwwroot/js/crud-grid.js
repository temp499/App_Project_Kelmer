const grid = document.querySelector("#crud-grid");
const form = document.querySelector("#crud-form");
const modal = new bootstrap.Modal("#crud-modal");
const config = grid.dataset;
const status = document.querySelector("#grid-message");
const formError = document.querySelector("#crud-error");

function showStatus(message, error = false) {
    status.textContent = message;
    status.className = `alert alert-${error ? "danger" : "success"}`;
}

function itemUrl(path, id) {
    const url = new URL(path, location.href);
    url.searchParams.set("id", id);
    return url;
}

const columns = [...grid.querySelectorAll("th[data-field]")].map(header => ({
    data: header.dataset.field,
    defaultContent: "—",
    render: $.fn.dataTable.render.text()
}));
columns.push({
    data: null,
    searchable: false,
    orderable: false,
    render: () => [
        config.edit === "true" ? '<button type="button" class="btn btn-sm btn-outline-primary me-1" data-edit>Editar</button>' : "",
        config.delete === "true" ? '<button type="button" class="btn btn-sm btn-outline-danger" data-delete>Borrar</button>' : ""
    ].join("") || "—"
});

const table = $(grid).DataTable({
    processing: true,
    serverSide: true,
    searchDelay: 300,
    order: [[1, "asc"]],
    ajax: {
        url: config.list,
        type: "POST",
        headers: { RequestVerificationToken: appRequests.token() },
        error: response => showStatus(appRequests.statusMessage(response.status), true)
    },
    columns,
    language: {
        search: "Buscar:",
        lengthMenu: "Mostrar _MENU_ registros",
        info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
        infoEmpty: "Sin registros",
        infoFiltered: "(de _MAX_ registros)",
        zeroRecords: "Sin resultados",
        emptyTable: "Sin registros",
        processing: "Cargando…",
        paginate: { previous: "Anterior", next: "Siguiente" }
    }
});

function openEditor(item) {
    form.reset();
    formError.classList.add("d-none");
    for (const input of form.querySelectorAll("[name]")) {
        if (item)
            input.value = item[input.dataset.source ?? input.name] ?? "";
    }
    document.querySelector("#crud-title").textContent = `${item ? "Editar" : "Nuevo"} ${config.entity}`;
    modal.show();
}

document.querySelector("[data-create]")?.addEventListener("click", () => openEditor());
grid.querySelector("tbody").addEventListener("click", async event => {
    const button = event.target.closest("button");
    if (!button) return;
    const item = table.row(button.closest("tr")).data();
    button.disabled = true;
    try {
        if (button.hasAttribute("data-edit")) {
            openEditor(await appRequests.send(itemUrl(config.item, item.id)));
        } else if (confirm(`¿Borrar el registro de ${config.entity}?`)) {
            const result = await appRequests.send(itemUrl(config.deleteUrl, item.id), "POST");
            showStatus(result.message);
            table.ajax.reload();
        }
    } catch (error) {
        showStatus(error.message, true);
    } finally {
        button.disabled = false;
    }
});

form.addEventListener("submit", async event => {
    event.preventDefault();
    const submit = form.querySelector('button[type="submit"]');
    submit.disabled = true;
    formError.classList.add("d-none");
    const fields = Object.fromEntries(new FormData(form));
    for (const input of form.querySelectorAll("[data-number]"))
        fields[input.name] = Number(input.value);
    fields.id = fields.id ? Number(fields.id) : null;
    try {
        const result = await appRequests.send(fields.id ? config.updateUrl : config.createUrl, "POST", fields);
        modal.hide();
        showStatus(result.message);
        table.ajax.reload(null, false);
    } catch (error) {
        formError.textContent = error.message;
        formError.classList.remove("d-none");
    } finally {
        submit.disabled = false;
    }
});
