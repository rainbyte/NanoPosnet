---
title: Full-stack usando F# y Saturn
author: Alvaro F. García
---

## Objetivo

Luego de una comparación preliminar, con énfasis en la experiencia didáctica,
se encontró que F# sobre .NET Core es una de las combinaciones más adecuadas
para la enseñanza y el desarrollo de sistemas full-stack.

Este trabajo busca ir más allá del análisis inicial, profundizando en los
aspectos específicos de la herramienta, e implementando una prueba de
concepto aplicada a un caso de uso real.

## Introducción

### .NET Core

Cabe destacar que .NET Core es un ecosistema basado en software libre,
publicado por Microsoft en Junio del 2016 bajo la Licencia MIT, con soporte
para varias plataformas y multiples lenguajes de programación.

Al ser una solución integral, abarca desde la gestión de dependencias hasta
un compilador JIT y el en entorno de ejecución CLR, que a diferencia de una
maquina virtual, ejecuta programas de forma nativa.

.NET Core además soporta 3 lenguajes principales: C#, VB.net, y F#, siendo
este ultimo el que vamos a estudiar con detenimiento.

### F#, el lenguaje

También llamado FSharp, forma parte de la familia de lenguajes ML, inspirado
inicialmente en OCaml, con cierto grado de compatibilidad y características
habituales de este grupo, especialmente la inferencia de tipos.

F# es multiparadigma, dando soporte a varios estilos de programación, entre los
cuales podemos nombrar: Funcional, Imperativo, Orientado a Objetos, Asíncrono,
Paralelo, Meta-programación.

La sintaxis del mismo es limpia y ergonómica, por lo cual es posible relacionar
en poco tiempo el conocimiento previo con la manera de expresarlo en F#, dando
incluso mayor comodidad.

### Preparación

Antes de usar .NET Core y F# debemos instalarlos, para lo cual podemos ingresar
a [este link](https://dotnet.microsoft.com/download) para descargar tanto su
Runtime como su SDK. En este momento la ultima versión publicada es la 3.1, y
la misma ya incluye los lenguajes oficiales en su interior.

En algunas distribuciones de Linux es posible usar los repositorios oficiales
para la instalación, en el caso de Arch Linux ejecutamos `pacman -S dotnet-sdk`.
Como vamos a utilizar componentes para desarrollo web, también necesitaremos
conseguir el paquete `aspnet-runtime`, que nos provee los mismos.

Por ultimo es recomendable descargar un plug-in para el editor de texto que
vayamos a utilizar. En el caso del editor multiplataforma
[Visual Studio Code](https://code.visualstudio.com/), podemos instalar la
extensión [Ionide](http://ionide.io/).

## Desarrollo

### Conceptos de F# para comenzar

Existen dos maneras de trabajar con F#. Por un lado podemos crear archivos de
extensión `.fsx`, los cuales se pueden ejecutar como scripts. Por otra parte
podemos crear en un directorio un proyecto de F# completo, donde hay:

- Un archivo `.fsproj` que describe el proyecto y sus dependencias.
- Uno o más archivos de código con extensión `.fs`, siendo usual llamar
`Program.fs` al principal.
- Un directorio `public`, con archivos necesarios para un front-end web.

Un archivo `Program.fs` como mínimo debe contener lo siguiente:

```fsharp
[<EntryPoint>]
let main args =
    printfn "%s" "hola"
    0 // exit success
```

Luego podemos utilizar el comando `dotnet` para compilar nuestro proyecto
y ejecutarlo:

```sh
dotnet build
dotnet run
```

### Sintaxis de F# con ejemplos

Las definiciones en F# son inmutables por default, e indicar el tipo de datos
es opcional, dado que el lenguaje soporta inferencia de tipos. A pesar de esto
podemos indicar si queremos permitir mutabilidad y usar la sintaxis `<-` para
realizar asignaciones. El signo `=` se asemeja a la igualdad matemática.

```fsharp
let a : int = 10 // inmutable by default
let b : float = 3.0
let s : string = "foo"

let mutable x = 5
x <- x + 1
```

F# también utiliza la keyword `let` para definir funciones, y es posible indicar
el tipo de datos agregando paréntesis. En la siguiente función se puede apreciar
el uso de condicionales, pero F# los trabaja como expresiones, por lo tanto el
resultado de la condición termina siendo el resultado de la función.

```fsharp
let test (x : int) y =
    if x = y then "iguales"
    elif x > 0 then "x es mayor"
    else "x es menor"
```

Es posible utilizar expresiones de **Pattern Matching** dentro de las funciones,
permitiendo trabajar con los valores de forma estructural y agregar condiciones
especificas en cada caso. Se utiliza la keyword `_` para indicar es resto de los
valores posibles, ya que es obligatorio cubrir todas las combinaciones.

```fsharp
let test (x : int) =
    match x with
    | 0            -> "cero"
    | n when n > 0 -> "positivo"
    | _            -> "negativo"
```

F# permite iterar de la forma tradicional imperativa, usando for loops en
rangos explícitos.

```fsharp
for i = 1 to 10 do
    printf "%d" i
```

También se pueden usar while-loops para iterar, combinándolos con variables
mutables para la condición de finalización.

```fsharp
let mutable z = 1
while z <= 10 do
    printfn "%d " z
    z <- z + 1
```

F# soporta Arrays de acceso indexado, y es posible iterar sobre ellos.

```fsharp
let a = [| 10; 20; 30; 40 |]
for x in d do
    printfn "%d" x
```

Otra característica importante de F# es la posibilidad de trabajar aplicando
el paradigma funcional con mucha comodidad. Se puede definir funciones simples
de manera concisa.

```fsharp
let inc x = x + 1
let doble x = x * 2
```

Luego podemos combinar dichas funciones utilizando **Composición**, en el orden
que nos sea conveniente.

```fsharp
let f = inc >> doble // incremento 1ro
let r1 = f 2 // r1 = (2 + 1) * 2 = 6

let g = inc << doble // multiplico 1ro
let r2 = g 2 // r2 = (2 * 2) + 1 = 5
```

Una alternativa extra para combinar funciones es usar **Pipelines**, pasando los
valores como si estuviéramos trabajando en un interprete Bash.

```fsharp
let r3 = 2 |> inc |> doble // r = 6
```

También tenemos flexibilidad para realizar pasaje de funciones como parámetro,
lo cual permite implementar código más modular, evitando repeticiones.

```fsharp
//  l = [2;3;4]
let l = List.map (fun x -> x + 1) [1;2;3]
```

- Union discriminada

```fsharp
type Booleano = Falso | Verdadero

let b : Booleano = Verdadero
```

- Enums C-like

```fsharp
type Nivel = Bajo = 1 | Medio = 2 | Alto = 3
let niv = Nivel.Bajo
```

Es posible utilizar tanto **tuplas** como **registros** para combinar elementos
de multiples tipos de datos, lo cual se conoce como **producto de tipos**.

```fsharp
let a : int * string = 1,"hola"
let b = 1,2
```

```fsharp
type Product = {
    cod:CodigoProducto;
    precio:float
}
```

Es posible aplicar las propiedades de producto y suma de tipos, lo cual se
conoce como **Tipos Algebraicos**, y permite tener la composición justa para
permitir solo instancias validas.

```fsharp
type Opcional<'t> = Nada | Algo of 't

let a : Opcional<'t> = Nada
let b : Opcional<int> = Algo 9

let numOrZero optNum =
    match optNum with
    | Nada   -> 0
    | Algo n -> n

let c = numOrZero a // c = 0
let d = numOrZero b // d = 9
```

F# también soporta la utilización de clases para aplicar la programación
orientada a objetos. Las definiciones que usan `let` van arriba, mientras
que los métodos van abajo usando la keyword `member`. Es importante notar
como los atributos mutables deben agregarse manualmente, ya que F# favorece
por default la inmutabilidad.

Otro aspecto interesante es que uno puede usar el nombre más conveniente
para acceder a la instancia del objeto, indicandolo antes del nombre del
metodo en su definición, ya sea `this.Metodo`, `x.Metodo`, `self.Metodo`,
en este caso lo llamamos `p`.

```fsharp
type Person(nom: string,
        ape: string, edad: int) =
    let mutable mApe = ape // backing store

    member p.Nombre = nom
    member p.Apellido
        with get() = mApe
        and set(value) = mApe <- value
    member val Edad = edad with get, set
    member p.NombreCompleto =
        p.Nombre + " " + p.Apellido

let p = new Person("foo", "bar", 50)
printfn "%s" p.NombreCompleto
p.Edad <- 60 // update value
printfn "%d" p.Edad
```

Es usual usar las clases junto con interfaces asociadas, evitando acoplamiento.
Al implementar un interface dentro de la clase, usamos escribimos `interface`.
Los miembros de un interface se indican como `abstract`.

```fsharp
type IFigura =
    abstract Pos : float * float
    abstract Area : float
    abstract Perim : float

type IPosDistance<'t when 't :> IFigura>
    abstract PosDistance : 't -> float

type Rectangulo(x:float, y:float,
        w:float, h:float) =
    interface IFigura with
        member r.Pos = (x, y)
        member r.Area = w * h
        member r.Perim = w * 2 + h * 2
    interface IPosDistance with
        member r.PosDistance(other) =
            let (ox,oy) = other.Pos
            sqrt((ox - x)**2 + (oy - y)**2)
```

Agregar datos a un archivo de texto puede realizarse con la ayuda de pipelines,
simplificando el proceso.

```fsharp
open System
open System.IO
let path = @"/tmp/foo.txt"
let data = [| "lala,foo,bar" |]

path
|> File.ReadAllLines
|> Array.append data
|> (fun x -> File.WriteAllLines(path,x))
```

### Bases de Datos usando Dapper

Para conectarnos a una base de datos SQL y realizar consultas, usaremos
Dapper y un database provider.

En el siguiente ejemplo mostramos como acceder a MariaDB usando el provider
MySql.Data, al cual debemos pasar la configuración de conexión como parámetro
y nos devolverá una instancia.

Para hacer la consulta tenemos el método `Query`, al cual le pasamos un string
SQL marcando los parámetros con `@` y un diccionario con los valores.
La función `box` actúa como wrapper, para datos de distinto tipo.

```fsharp
open Dapper
open MySql.Data.MySqlClient

type Persona = { nom: string; edad: int}

let dbConfig =
    "Server=localhost; Port=1234;\
     Uid=usuario; Pwd=clave"

let personaIndex =
    let db = new MySqlConnection(dbConfig)
    let sql = "SELECT * FROM personas\
               WHERE edad=@edad"
    let data = dict ["edad", box 18]
    db.Query<Persona>(sql, data)
```

### Sistemas Web usando Saturn y Giraffe

Al desarrollar sistemas Web, tenemos disponible una combinación de software
llamada **SAFE**, que en F# es equivalente al tradicional *LAMP*. El conjunto
SAFE tiene como componente principal a **Saturn**, el cual se basa en Giraffe
para proveer un servidor web agregando funcionalidad extra.

F# provee las llamadas `Computation Expressions`, que permiten un modelo de
trabajo basado en pequeños lenguajes de propósito especifico, de manera tal
que el compilador chequea que ciertas reglas se cumplan dentro de estas
expresiones. *Saturn* las usa en sus capas de abstracción.

Una de las abstracciones se llama `Application`, la cual permite configurar
los parámetros generales de *Saturn* como servidor y una función router que
explicaremos en un momento. Aquí podemos ver como se implementa esto en un
contexto de `Computation Expressions`, usando la sintaxis de F# con forma
`builder { expressions }`. Dentro de las llaves el compilador chequea que
se cumplan las pautas definidas en el tipo `Application`.

```fsharp
let app = application {
    use_router routerMain
    use_cors "CORS" (fun builder ->
        builder.WithOrigins("*")
            .AllowAnyMethod()
            .WithHeaders("content-type")
        |> ignore)
    service_config (fun s -> s.AddGiraffe())
}

[<EntryPoint>]
let main _ =
    run app
    0 // exit code
```

Mencionamos que debe indicarse una función de tipo `Router` en la configuración,
la cual tiene importancia ya que contiene la lógica para manejar los requests en
el servidor, marcando los handlers correspondientes.

`Router` también se escribe mediante `Computation Expressions`, teniendo allí
disponibles operaciones para los métodos http (post, get, etc) y la operación
`forward` que permite delegar requests a otro `Router`.

Nuevamente el compilador hará los chequeos correspondientes, respetando la
implementación del tipo de datos `Router`, guiándonos a un uso correcto de
sus operaciones.

```fsharp
let routerMain = router {
    get "/" indexAction
    get "/cobro" cobroAction
    forward "/txs" txController
    forward "/nodo" routerNodo
}
```

Finalmente la última capa de importancia es el tipo `Controller`, que permite
definir un recurso web al estilo API REST mediante `Computation Expressions`,
donde indicaremos las funciones que manejaran cada tipo de endpoint.

Dentro de las llaves podemos definir mediante funciones los siguientes casos:
index, show, add, edit, create, update, patch, delete, deleteAll.

En cada uno que indiquemos debemos procesar el request según corresponda,
consumiendo el contenido de `Context` y los parámetros del recurso si los
hubiese, para generar asi un response valido.

```fsharp
let txController = controller {
    index (fun ctx ->
        Controller.json ctx store)

    show (fun ctx factura ->
        find factura |> Controller.json ctx)

    create (fun ctx -> task {
        let! input =
            Controller.getModel<Tx> ctx
        store.Add input
        return! Controller.json ctx input
    })
}
```

## Conclusión

F# nos provee muchas herramientas dentro del lenguaje para diseñar nuestro
sistema de manera muy cómoda, recibiendo ayuda del compilador para detectar
y solucionar los problemas que aparezcan.

Saturn específicamente aprovecha toda esta maquinaria para ir un paso más
allá y chequear usando pequeños DSLs dentro de `Computation Expressions`
que la implementación sea correcta.

Luego de compilar el código tenemos un alto nivel de confiabilidad en la
implementación, y podemos centrarnos solo en el dominio del problema.

## Bibliografía

- [https://dev.to/kspeakman/dirt-simple-sql-queries-in-f-a37](https://dev.to/kspeakman/dirt-simple-sql-queries-in-f-a37)
- [https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions)
- [https://dotnetperls.com/](https://dotnetperls.com/)
- [https://en.wikibooks.org/wiki/F_Sharp_Programming/Computation_Expressions](https://en.wikibooks.org/wiki/F_Sharp_Programming/Computation_Expressions)
- [https://fsharp.github.io/FSharp.Data/library/Http.html](https://fsharp.github.io/FSharp.Data/library/Http.html)
- [https://fsharp.org/guides/web/](https://fsharp.org/guides/web/)
- [https://fsharpforfunandprofit.com/posts/computation-expressions-intro/](https://fsharpforfunandprofit.com/posts/computation-expressions-intro/)
- [http://fssnip.net/a7/title/Send-HTTP-POST-request](http://fssnip.net/a7/title/Send-HTTP-POST-request)
- [https://gist.github.com/rainbyte/36620a1288424659d60c65ca19c04603](https://gist.github.com/rainbyte/36620a1288424659d60c65ca19c04603)
- [https://github.com/giraffe-fsharp/Giraffe](https://github.com/giraffe-fsharp/Giraffe)
- [https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md](https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md)
- [https://github.com/haf/Http.fs](https://github.com/haf/Http.fs)
- [https://github.com/SaturnFramework/Saturn](https://github.com/SaturnFramework/Saturn)
- [https://github.com/StackExchange/Dapper](https://github.com/StackExchange/Dapper)
- [https://isthisit.nz/posts/2019/sqlite-database-with-dapper-and-fsharp/](https://isthisit.nz/posts/2019/sqlite-database-with-dapper-and-fsharp/)
- [https://jeremypresents.azurewebsites.net/saturn.html#/](https://jeremypresents.azurewebsites.net/saturn.html#/)
- [http://kcieslak.io/Magic-of-Saturn-controllers](http://kcieslak.io/Magic-of-Saturn-controllers)
- [https://nozzlegear.com/blog/using-dapper-with-fsharp](https://nozzlegear.com/blog/using-dapper-with-fsharp)
- [https://safe-stack.github.io/](https://safe-stack.github.io/)
- [https://safe-stack.github.io/docs/](https://safe-stack.github.io/docs/)