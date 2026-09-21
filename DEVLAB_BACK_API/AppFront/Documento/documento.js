const URL_API = 'https://localhost:7081/api/v1/Documento';

async function enviarDocumento() {

    const codigoCliente = document.getElementById('codigoCliente').value;
    const arquivo = document.getElementById('arquivo').files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o código do cliente e selecione um arquivo.");
        return;
    }

    const formData = new FormData();

    formData.append("arquivo", arquivo);

    const response = await fetch(
        `${URL_API}/upload/${codigoCliente}`,
        {
            method: 'POST',
            body: formData
        }
    );

    if (response.ok) {

        const resultado = await response.json();

        alert(resultado.mensagem);

        document.getElementById('codigoCliente').value = '';
        document.getElementById('arquivo').value = '';

    } else {

        const erro = await response.json();

        alert(erro.erro || "Erro ao enviar o documento.");
    }
}


async function listarDocumentos() {

    const codigoCliente =
        document.getElementById('codigoClienteBusca').value;

    if (!codigoCliente) {
        alert("Informe o código do cliente.");
        return;
    }

    const response =
        await fetch(`${URL_API}/listar/${codigoCliente}`);

    const tabela =
        document.getElementById('tabelaDocumentos');

    tabela.innerHTML = '';

    if (!response.ok) {

        const erro = await response.json();

        alert(erro.mensagem || "Nenhum documento encontrado.");

        return;
    }

    const documentos = await response.json();

    documentos.forEach(documento => {

        tabela.innerHTML += `
            <tr>
                <td>${documento.id}</td>
                <td>${documento.name}</td>
                <td>${documento.extensao}</td>
                <td>
                    <button class="btn"
                        onclick="baixarDocumento(${documento.id})">
                        Baixar
                    </button>

                    <button class="btn"
                        onclick="excluirDocumento(${documento.id})">
                        Excluir
                    </button>
                </td>
            </tr>
        `;
    });
}


function baixarDocumento(id) {

    window.open(
        `${URL_API}/download/${id}`,
        '_blank'
    );
}


async function excluirDocumento(id) {

    if (!confirm("Deseja realmente excluir este documento?")) {
        return;
    }

    const response =
        await fetch(`${URL_API}/excluir/${id}`, {
            method: 'DELETE'
        });

    if (response.ok) {

        alert("Documento excluído com sucesso.");

        listarDocumentos();

    } else {

        alert("Erro ao excluir o documento.");
    }
}