//Igual que C# fai ilegal gardar un integer nun campo string, F# leva isto un paso mais ala, e permitenos comenzar o modelo das reglas de negocio no codigo, co cal e imposible representar un illegal state. Isto fai os tests unitarios moito mais simples, ou idealmente poden ser completamente omitidos

//Supoñamos que queremos modelar os detalles de contacto dun cliente en codigo que se adhire a unha cantidade de regras simples
//Un record de F# representando un cliente de exemplo
type Cliente =
    { IdCliente : string
      Email : string
      Telefono : string
      Direccion : string }
//Gardando todos os posibles detalles do contacto como tres campos separados
//Calquera destos campos poderian conter algo ou non

//Mezclando valores do mesmo tipo
//Algo que chama a atencion e que no exemplo anterior todos os campos usan o mesmo type: un string
//Exemplo dunha funcion que pode crear un cliente
let crearCliente idCliente email telefono direccion =
    { IdCliente = telefono 
      Email = idCliente 
      Telefono = direccion 
      Direccion = email }
  let cliente =
    crearCliente "C-123" "meridio@meuemail.com" "874 293 823" "Rua Choco 7"

//Aqui mezclamos as asignacions para cada un dos campos (fileds). Dado que estamos usando o mismo type de exemplo, string, para todos os campos, o compilador non dara erro. 
//Probablemente atopemos o erro se escribiches algunhas unit tests, ou quizais cando comprobes a base de datos e vexas que estas almacenando datos nas columnas equivocadas.

//Single-case Discriminated Unions
//F# ten unha bonita forma de resolver isto, chamada single-case DUs. En que axuda ter unha DU con unha unica simple posibilidade? Pois porque a sintaxe simple dunha DU, permite usar simples wrapper classes para evitar mezclar valores accidentalmente

//Creando unha wrapper type (ou class) usando unha single-case DU
type Direccion = Direccion of string //Creando unha single-case DU para gardar un string Direccion
let minhaDireccion = Direccion "Rua Choco 7" //Crendo unha isntancia dunha Direccion wrapped
let eMesmaDireccion = (minhaDireccion = "Rua Choco 7") //Comparar unha Direccion wrapped e un string puro non compila
let (Direccion datosDireccion) = minhaDireccion //Unwrapping unha Direccion no seu string puro como datosDireccion

// 1 Comezamos creando o rexistro de cliente e a función crearClieete
// (coas asignacions incorrectas).
// 2 Crea catro unións discriminadas single-case, unha para cada tipo de string que teñas
// quere almacenar (IdCliente, Email, Telefono e Direccion).
// 3 Actualiza a definición do type Cliente para que cada campo use o tipo de wrapper -envoltura- correcto. Asegúrate de definir os tipos de envoltorios antes que o type Cliente.
// 4 Actualiza o callsite de crearCliente para que envolva cada valor de entrada no DU correcto; terás que rodear cada "envoltura" (wrapper) entre parénteses.
// Se o fixeches correctamente, notarás que o teu código deixa de funcionar.
// Curiosamente, o erro do compilador xerarase no callsite para chamar a crearCliente.
// Este é un caso de "seguir o pan relado" con type inference; se levas o rato
// sobre calquera dos argumentos da función en si, verás que isto é porque
// mesturaches os assingments cos campos equivocados.
// 5 Corrixe os assingments na función crearCliente e verás como por arte de maxia
// desaparecen todos os erros. 
type IdCliente = IdCliente of string //Creando varios single-case DUs
type Email = Email of string 
type Telefono = Telefono of string 
type Cliente =
    { IdCliente : IdCliente 
      Email : Email
      Telefono : Telefono
      Direccion : Direccion } //Direccion xa a temos como DU enriba, por iso non a creamos aqui

let crearCliente idCliente email telefono direccion =
    { IdCliente = idCliente 
      Email = email //usando os single-case DUs no tipo Cliente
      Telefono = telefono 
      Direccion = direccion }
let cliente       
    crearCliente
        (IdCliente "C-123") //Un single-case DU pode protexer de asignar valores incorrectos
        (Email "meridio@meuemail.com")
        (Telefono "874 293 823")
        (Direccion "Rua Choco 7")

//E boa practica envolver sempre un valor nun DU en canto se presente a oportunidade, por exemplo cando leas datos dun arquivo de texto ou base de datos.

// Combinamos os tres DUs de caso único nun DU de tres casos chamado DetallesDeContacto
// e cambiamos o type Cliente para almacenalo asi e non nun campo para cada tipo de detalle de contacto:
// escriba detalles de contacto =

// 6 Substitúe os tres DU single-case polo novo tipo DetallesDeContacto.
// 7 Actualiza o tipo de cliente substituíndo os tres campos opcionais por un único campo
// do tipo DetallesDeContacto.
// 8 Actualiza a función crearCliente. Agora só ten que ter dous argumentos,
// o IdCliente e os DetallesDeContacto.
// 9 Actualiza o lugar de chamada segundo corresponda; por exemplo:
// let cliente =
//   crearCliente (IdCliente "Pamela") (Email "pamguachapun@pamemail.com")
// Agora podes garantir que se engade un e só un tipo de contacto (por exemplo,
// Telefono). 
type DetallesDeContacto=
    | Direccion of string
    | Telefono of string
    | Email of string
type IdCliente = IdCliente of string

  
//Usando valores opcionais (optional values) nun domain
// O seguinte requisito debe ser bastante sinxelo:
// Os clientes deben ter un detalle de contacto principal obrigatorio e un detalle de contacto secundario opcional.

// 10 Engadimos un novo campo ao cliente que conteña un detalle de contacto opcional e
// renomeamos o campo de DetallesDeContacto orixinal a DetallesDeContactoPrimarios. 
type Cliente = 
    { IdCliente : IdCliente
      DetallesDeContactoPrimarios : DetallesDeContacto 
      DetallesDeContactoSecundarios : DetallesDeContacto option}
// 11 Actualizamos a funcion crearCliente e o seu callsite de maneira apropiada
let crearCliente idCliente detallesDeContacto detallesSecundarios =  
    { IdCliente = idCliente 
      DetallesDeContactoPrimarios = detallesDeContacto 
      DetallesDeContactoSecundarios = detallesSecundarios}
let cliente = crearCliente (IdCliente "C-123") (Email "meridio@meuemail.com") None

//Este foi un cambio sinxelo
//Suponhamos agora que no cambio final, supostamente o mais desafiante e interesante
// Os clientes deben ser validados como clientes auténticos, baseado en se o seu detalle de contacto principal é un enderezo de correo electrónico dun dominio específico. Só cando os clientes pasaron por este proceso de validación reciben un correo electrónico de benvida. Ten en conta que será tamén precisa realizar máis funcionalidades no futuro, dependendo de se un cliente é xenuíno 
//  12 crea unha función chamada ValidarCliente que toma un cliente descoñecido sen tratar, e devolve un cliente novo e xenuíno, un novo tipo que podes tratar de xeito diferente do cliente sen tratar. Ao facelo, podes distinguir entre un cliente non validado e un que se confirmou como auténtico. Como verás en breve, isto pode ser útil de moitas maneiras, pero primeiro vexamos como se ve no código. 

//Creamos un DU single-case que funciona como un type marcador (marker type); envolve a un Cliente estandar, e permiteche tratalo de maneira distinta

//DU single-case que envolve o tipo Cliente
type ClienteAutentico = ClienteAutentico of Cliente  

//Creando unha funcion para crear un rating dun cliente
///Funcion que toma un Cliente normal, non validado e crear un ClienteAutentico opcional como saida
let validarCliente cliente =
    match cliente.DetallesDeContactoPrimarios with 
    | Email e when e.EndsWith "hotmail.com" -> Some(ClienteAutentico cliente) //loxica custom para validar un cliente
    | Direccion _ | Telefono _ -> Some(ClienteAutentico cliente) //facendo wrapping a un cliente validado como Autentico
    | Email _ -> None
let enviarEmailDeBenvida (ClienteAutentico cliente) = //O metodo enviarEmailDeBenvida so acepta ClienteAutentico como entrada
    printfn "Hola, %A, e benvido como cliente que pagaras ben o carto" cliente.IdCliente 

enviarEmailDeBenvida cliente //non compila

//compila - so se chama se validarCliente devolve Some(ClienteAutentico _)
cliente 
|> validarCliente
|> Option.map enviarEmailDeBenvida   

// Cando e cando non usar tipos de marcador (marker types)
// A creación de tipos de marcador pode ser increíblemente poderosa. Podes usalos para todo tipo de cousas. Por exemplo, imaxina ser capaz de definir enderezos de correo electrónico verificados ou non para os teus usuarios. Ou que tal distinguir entre os estados dun
// pedido no type system (por exemplo, sen pagar, pagado, enviado ou completado)? Poderías
// ter funcións que actúan só baixo encargos completados e non te preocupas de chamar accidentalmente a esa funcion a un pedido sen pagar! Tamén podes usalos para crear límites na tua aplicación, realizando a validación de datos non chequeados e converténdoos en  versións de datos chequeadas, que che proporcionan seguridade de que nunca podes executar certo código sobre datos non válidos.
// É aconsellable usar DUs single-case como envoltorios para evitar simples
// erros como mesturar identificacións de cliente e de pedido. É fácil de facer, e é un
// axuda inmensa para eliminar algúns erros terribles que poden aparecer. Levándoo máis lonxe cos tipos de marcador para representar estados son un paso adiante e definitivamente paga a pena perseverar. Pode eliminar toda clase de bugs, así como evitar unit tests.
// Pero ten coidado de non levalo demasiado lonxe, xa que pode ser difícil vadear por un mar de types se te excedes. 

//Exceptions vs Results
// En F #, podes usar excepcións como faría en C #, usando a sintaxe try .. with . É interesante mencionar que as excepcións non están codificadas dentro do sistema de types. Por exemplo, imaxinemos inserindo un cliente nunha base de datos. A sinatura pode ter o seguinte aspecto:
// insertaContacto: contactoDetalles: ContactoDetalles -> IdCliente
// Noutras palabras, dados os datos de contacto, gárdaos na base de datos e devolve o ID de cliente xerado. Pero esta función non responde á posibilidade de que a base de datos
// pode estar sen conexión ou que alguén con eses datos de contacto xa exista. De feito,
// alguén que mire este código só o sabería se houbese un controlador de try / catch
// nalgún lugar do código, que podería ser unha área completamente diferente da base do código. Isto pode considérarse como unha función non segura.
// Unha alternativa ao uso de excepcións é usar un result. Trátase dunha two-case discriminated union que resulta en éxito ou fracaso. Aquí, se a chamada pasa, devolverás un éxito co IdCliente xerado pola base de datos. Se falla, devolverá o texto do erro
// SQL como caso de fallo.

//Definindo unha discriminated union dun simple Result
type Result<'a> =
| Success of 'a
| Failure of string

let insertarContactoFormaInsegura detallesDeContacto =
    if detallesDeContacto = (Email "meridio@meuemail.com") then
        { IdCliente = IdCliente "ABC" 
          DetallesDeContactoPrimarios = detallesDeContacto 
          DetallesDeContactoSecundarios = None }
    else failwith "Incapaz de insertar o cliente - xa existe"

let insertarContactoFormaSegura detallesDeContacto =
    if detallesDeContacto = (Email "meridio@meuemail.com") then
        Success { IdCliente = IdCliente "ABC"
        DetallesDeContactoPrimarios = detallesDeContacto
        DetallesDeContactoSecundarios = None }
    else Failure "Incapaz de insertar o cliente - xa existe"

//manexando ambos casos de Success e Failure por adiantado
match insertarContactoFormaSegura detallesDeContacto =
    if detallesDeContacto (Email "meridio@meuemail") with   
    | Success cliente -> printfn "Gardado na base de datos con %A" cliente.IdCiente 
    | Failure error -> printfn "Non poidemos gardar o contacto: %s" error