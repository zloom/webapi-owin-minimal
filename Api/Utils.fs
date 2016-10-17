namespace Api

open System.Configuration
open System.Web.Http.Filters
open System.Web.Http.Controllers
open System.Reflection
open System.Runtime.InteropServices

module Utils = 

    type Opt = System.Runtime.InteropServices.OptionalAttribute

    type Def = System.Runtime.InteropServices.DefaultParameterValueAttribute

    let connection(name:string) =
        ConfigurationManager.ConnectionStrings.[name].ConnectionString


module Filters =     

    type DefaultParameterValueFilter() =
      inherit ActionFilterAttribute() 

      let defaultValues (parameters: ParameterInfo array) =
        [ for param in parameters do
            let attrs = List.ofSeq (param.GetCustomAttributes<DefaultParameterValueAttribute>())
            match attrs with
            | [x] -> yield param, x.Value
            | [] -> () 
            | _ -> failwithf "Multiple DefaultParameterValueAttribute on param '%s'!" param.Name
        ]
  
      let setDefaultValues (context: HttpActionContext) =
        match context.ActionDescriptor with
        | :? ReflectedHttpActionDescriptor as ad ->
          let defaultValues = defaultValues (ad.MethodInfo.GetParameters())
          for (param, value) in defaultValues do
            match context.ActionArguments.TryGetValue(param.Name) with
            | true, :? System.Reflection.Missing
            | false, _ ->
              let _ = context.ActionArguments.Remove(param.Name)
              context.ActionArguments.Add(param.Name, value)
            | _, _ -> ()
        | _ -> ()
  
      override __.OnActionExecuting(context) =
        setDefaultValues context
        base.OnActionExecuting(context)
  
      override __.OnActionExecutingAsync(context, cancellationToken) =
        setDefaultValues context
        base.OnActionExecutingAsync(context, cancellationToken)