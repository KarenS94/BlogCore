// aqui hacemos una funcion que se carga cuando el documento este listo.

var dataTable;

$(document).ready(function () {
    cargarDatatable();
});

function cargarDatatable() {
    dataTable = $("#tblSliders").DataTable({
        "ajax": {
            //pasamos los parametros
            "url": "/admin/slider/GetAll",// obtiene el url de la carp admin,controlador categoria, y el metodo GetAll formato json
            "type": "GET", // peticion de tipo Get
            "datatype": "json"
        },
        // aqui definimos la columnas
        "columns": [
            { "data":"id", "width": "5%" },
            { "data":"nombre", "width": "20%" },
            // EMPIEZA LA EDICION ESTADO
            {
                "data": "urlImagen",
                "render": function (estadoActual) {
                    if (estadoActual == true) {
                        return "Activo"

                    } else {
                        return "Inactivo"
                    }
                  
                }, "width": "15%"
            },
            // CIERRA EDICION ESTADO
             {
                "data": "urlImagen",
                "render": function (imagen) {
                   return `<img src="../${imagen}" width="120px">`
                 }, "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return`<div class="text-center">
                            <a href="/Admin/Slider/Edit/${data}" class="btn btn-success text-white" style="cursor:pointer; width:140px;">
                            <i class="far fa-edit"></i>  Editar
                            </a>
                            &nbsp;
                            <a onclick=Delete("/Admin/Slider/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer; width:140px;">
                            <i class="far fa-trash-alt"></i>  Borrar
                            </a>
                          </div>

                          `;
                }, "width": "20%"
            }

        ],
        "language": {
            "decimal": "",
            "emptyTable": "No hay información",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior" 
            }
        },
        "width": "5%"

    });
}

// aqui ponemos el codigo de sweeatalert

function Delete(url) {

    swal({
        title: '¿Estás seguro/a de borrar?',
        text: 'Este contenido no se puede recuperar!',
        type:'warning',
        showCancelButton: true,
        confirmButtonColor: '#DD6B55',
        confirmButtonText: 'Si, Borrar!',
        closeOnConfirm: true
       
    }, function () {
        $.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                }
                else {
                    toastr.error(data.message);
                }
            }
        });
    });
}