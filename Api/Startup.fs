namespace Api

open System.Web.Http

open global.Owin

open Newtonsoft.Json.Serialization

open Filters

type Startup() =

    member __.Configuration(app:Owin.IAppBuilder) =      
      
        let config = new HttpConfiguration()
        config.Formatters.Remove config.Formatters.XmlFormatter |> ignore 
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <- DefaultContractResolver() 
        config.MapHttpAttributeRoutes()
        config.Filters.Add(DefaultParameterValueFilter())     
     
        app.UseWebApi(config) |> ignore

