namespace Identity.Application.UnitTests

module Program =
    open Expecto
    
    [<EntryPoint>]
    let main argv =
        Tests.runTestsInAssembly defaultConfig argv