module Program

open FSharp.Control.Tasks.ContextInsensitive
open Giraffe
open Giraffe.GiraffeViewEngine
open Saturn

open Dapper
open MySql.Data.MySqlClient

open System.IO
open System.Net
open System.Text

type NanoAccount = string
type NanoBlock = string

type Tx = {
    factura: string;
    monto: string; // 128bit raw amount, 1 Nano = 1^30 raw
    detalle: string;
    bloque: NanoBlock;
}

let withDb cont =
    let credentials = "Server=localhost; Port=1028; Uid=nanoposnet; Pwd=7e67c3b668dbfcf69faf68447008bce9; Database=nanoposnet"
    let db: MySqlConnection = new MySqlConnection(credentials)
    cont db

let storeIndex = withDb (fun db -> 
    let sql = "SELECT * FROM Transacciones"
    db.Query<Tx>(sql))

let storeCreate (tx : Tx) : Unit = withDb (fun db ->
    let sql = "INSERT INTO Transacciones (monto, factura, detalle, bloque) VALUES (@monto, @factura, @detalle, @bloque)"
    let data = dict [
        "monto", box tx.monto
        "factura", box tx.factura
        "detalle", box tx.detalle
        "bloque", box tx.bloque ]
    db.Query<Tx>(sql, data) |> ignore)

let store = ResizeArray<Tx>()
let find (factura : string) = storeIndex |> Seq.tryFind (fun tx -> tx.factura.Equals factura)

let txController = controller {
    index (fun ctx -> storeIndex |> Controller.json ctx)

    show (fun ctx factura -> find factura |> Controller.json ctx)

    create (fun ctx -> task {
        let! input = Controller.getModel<Tx> ctx
        storeCreate input
        return! Controller.json ctx input })
}

let indexView =
    div [] [
        h2 [] [rawText "Nano Bookkeeper"]
    ]

let indexAction = htmlView indexView
let cobroAction = htmlFile "public/cobro.html"

let nanoverseRequest (data : string) =
    let url = "https://nanoverse.io/api/node"

    let req = HttpWebRequest.Create(url) :?> HttpWebRequest
    req.ProtocolVersion <- HttpVersion.Version11
    req.Method <- "POST"

    let postBytes = Encoding.ASCII.GetBytes(data)
    req.ContentType <- "application/json";
    req.ContentLength <- int64 postBytes.Length

    // Write data to the request
    let reqStream = req.GetRequestStream() 
    reqStream.Write(postBytes, 0, postBytes.Length);
    reqStream.Close()

    // Obtain response and download the resulting page 
    // (The sample contains the first & last name from POST data)
    let resp = req.GetResponse()
    let stream = resp.GetResponseStream()
    let reader = new StreamReader(stream)

    reader.ReadToEnd()

let nanoverseFrontier account =
    let data = sprintf "{%s}" account
    let res = nanoverseRequest data
    res

let routerNodo = router {
    post "/" (fun next ctx ->
        let url = "https://nanoverse.io/api/node"

        let body = ctx.Request.Body
        let reader = new StreamReader(body)
        let data = reader.ReadToEndAsync().Result

        printfn "Data:%s" data

        let res = nanoverseRequest data

        text res next ctx)
}

let routerMain = router {
    get "/" indexAction
    get "/cobro" cobroAction
    forward "/txs" txController
    forward "/nodo" routerNodo
}

let app = application {
    use_router routerMain
    use_cors "CORS" (fun builder -> builder.WithOrigins("*").AllowAnyMethod().WithHeaders("content-type") |> ignore)
    service_config (fun s -> s.AddGiraffe())
}

[<EntryPoint>]
let main _ =
    run app
    0 // exit code