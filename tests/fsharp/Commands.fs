[<RequireQualifiedAccess>]
module Commands

open System
open System.IO

open PlatformHelpers

let getfullpath workDir path =
    let rooted =
        if Path.IsPathRooted(path) then path
        else Path.Combine(workDir, path)
    rooted |> Path.GetFullPath

let fileExists workDir path = 
    if path |> getfullpath workDir |> File.Exists then Some path else None

let directoryExists workDir path = 
    if path |> getfullpath workDir |> Directory.Exists then Some path else None

/// copy /y %source1% tmptest2.ml
let copy_y workDir source to' = 
    log "copy /y %s %s" source to'
    File.Copy( source |> getfullpath workDir, to' |> getfullpath workDir, true)
    CmdResult.Success

/// mkdir orig
let mkdir_p workDir dir =
    log "mkdir %s" dir
    Directory.CreateDirectory ( Path.Combine(workDir, dir) ) |> ignore

/// del test.txt
let rm dir path =
    log "rm %s" path
    let p = path |> getfullpath dir
    if File.Exists(p) then File.Delete(p)

let pathAddBackslash (p: FilePath) = 
    if String.IsNullOrWhiteSpace (p) 
    then p
    else
        p.TrimEnd ([| Path.DirectorySeparatorChar; Path.AltDirectorySeparatorChar |]) 
        + Path.DirectorySeparatorChar.ToString()

// echo. > build.ok
let ``echo._tofile`` workDir text p =
    log "echo.%s> %s" text p
    let to' = p |> getfullpath workDir in File.WriteAllText(to', text + Environment.NewLine)

/// echo // empty file  > tmptest2.mli
let echo_tofile workDir text p =
    log "echo %s> %s" text p
    let to' = p |> getfullpath workDir in File.WriteAllText(to', text + Environment.NewLine)

/// echo // empty file  >> tmptest2.mli
let echo_append_tofile workDir text p =
    log "echo %s> %s" text p
    let to' = p |> getfullpath workDir in File.AppendAllText(to', text + Environment.NewLine)

/// type %source1%  >> tmptest3.ml
let type_append_tofile workDir source p =
    log "type %s >> %s" source p
    let from = source |> getfullpath workDir
    let to' = p |> getfullpath workDir
    let contents = File.ReadAllText(from)
    File.AppendAllText(to', contents)

let fsc exec (fscExe: FilePath) flags srcFiles =
    exec fscExe (sprintf "%s %s" flags (srcFiles |> Seq.ofList |> String.concat " "))

let csc exec cscExe flags srcFiles =
    exec cscExe (sprintf "%s %s"  flags (srcFiles |> Seq.ofList |> String.concat " "))

let fsi exec fsiExe flags sources =
    exec fsiExe (sprintf "%s %s" flags (sources |> Seq.ofList |> String.concat " "))

let msbuild exec msbuildExe flags srcFiles =
    exec msbuildExe (sprintf "%s %s"  flags (srcFiles |> Seq.ofList |> String.concat " "))

let resgen exec resgenExe flags sources =
    exec resgenExe (sprintf "%s %s" flags (sources |> Seq.ofList |> String.concat " "))

let internal quotepath (p: FilePath) =
    let quote = '"'.ToString()
    if p.Contains(" ") 
    then (sprintf "%s%s%s" quote p quote)
    else p

let ildasm exec ildasmExe flags assembly =
    exec ildasmExe (sprintf "%s %s" flags (quotepath assembly))

let peverify exec peverifyExe flags path =
    exec peverifyExe (sprintf "%s %s" (quotepath path) flags)

let createTempDir () =
    let path = Path.GetTempFileName ()
    File.Delete path
    Directory.CreateDirectory path |> ignore
    path

let convertToShortPath path =
    log "convert to short path %s" path
    let result = ref None
    let lastLine = function null -> () | l -> result := Some l

    let cmdArgs = { RedirectOutput = Some lastLine; RedirectError = None; RedirectInput = None }
    
    let args = sprintf """/c for /f "delims=" %%I in ("%s") do echo %%~dfsI""" path

    match Process.exec cmdArgs (Path.GetTempPath()) Map.empty "cmd.exe" args with
    | ErrorLevel _ -> path
    | CmdResult.Success -> match !result with None -> path | Some p -> p

let where envVars cmd =
    log "where %s" cmd
    let result = ref None
    let lastLine = function null -> () | l -> result := Some l

    let cmdArgs = { RedirectOutput = Some lastLine; RedirectError = None; RedirectInput = None; }

    match Process.exec cmdArgs (Path.GetTempPath()) envVars "cmd.exe" (sprintf "/c where %s" cmd) with
    | ErrorLevel _ -> None
    | CmdResult.Success -> !result    

let fsdiff exec fsdiffExe file1 file2 =
    // %FSDIFF% %testname%.err %testname%.bsl
    exec fsdiffExe (sprintf "%s %s normalize" file1 file2)

