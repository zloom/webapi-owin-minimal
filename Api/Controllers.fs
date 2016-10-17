namespace Api

open System.Web.Http

[<RoutePrefix("api/test")>]
type LoginController() = 
    inherit ApiController()    
    
    [<Route("")>]
    member __.Get() = "Hello!"
     


   



   

   
