$(document).ready(function () {
    // Llamada AJAX a la API para obtener la lista de departamentos
    $.ajax({
        url: '/api/departamento',  // Ruta de la acción del controlador
        type: 'GET', // Método de solicitud
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('token') // Si usas autenticación con JWT
        },
        success: function (data) {
            // La respuesta del API es almacenada en 'data'
            console.log(data); // Puedes imprimir los datos en la consola para verificarlos
            // Aquí puedes manipular los datos para mostrarlos en tu interfaz de usuario
            mostrarDepartamentos(data);
        },
        error: function (xhr, status, error) {
            // En caso de error
            console.error('Error: ', error);
            alert('Hubo un error al obtener los departamentos.');
        }
    });
});

// Función para mostrar los departamentos en el HTML
function mostrarDepartamentos(departamentos) {
    var listaHTML = '';
    departamentos.forEach(function (departamento) {
        listaHTML += '<tr>';
        listaHTML += '<td>' + departamento.idCliente + '</td>';
        listaHTML += '<td>' + departamento.nombre + '</td>';
        listaHTML += '<td>' + departamento.idTipoDepartamento + '</td>';
        listaHTML += '<td>' + departamento.precio + '</td>';
        listaHTML += '<td>' + departamento.numeroHabitaciones + '</td>';
        listaHTML += '<td>' + departamento.estado + '</td>';
        listaHTML += '<td><a href="/Departamento/Edit/' + departamento.idDepartamento + '">Edit</a> | <a href="/Departamento/Details/' + departamento.idDepartamento + '">Details</a> | <a href="/Departamento/Delete/' + departamento.idDepartamento + '">Delete</a></td>';
        listaHTML += '</tr>';
    });
    $('#departamentosList table').html(listaHTML); // Agregar los departamentos a la tabla
}
