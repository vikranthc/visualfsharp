module ``FSharp-Tests-Typecheck``

open System
open System.IO
open NUnit.Framework

open FSharpTestSuiteTypes
open NUnitConf
open PlatformHelpers
open SingleTest


module ``Full-rank-arrays`` = 

    [<Test; FSharpSuiteScriptPermutations("typecheck/full-rank-arrays")>]
    let ``full-rank-arrays`` p = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        // %CSC% /target:library /out:HighRankArrayTests.dll .\Class1.cs
        do! csc cfg "/target:library /out:HighRankArrayTests.dll" ["Class1.cs"]

        do! SingleTest.singleTestBuild cfg p
        
        do! SingleTest.singleTestRun cfg p
        })


module Misc = 

    [<Test; FSharpSuiteScriptPermutations("typecheck/misc")>]
    let misc p = singleTestBuildAndRun p

module Sigs = 

    [<Test; FSharpSuiteTest("typecheck/sigs")>]
    let sigs () = check (attempt {
        let cfg = FSharpTestSuite.testConfig ()

        do! fsc cfg "%s --target:exe -o:pos24.exe" cfg.fsc_flags ["pos24.fs"]
        do! peverify cfg "pos24.exe"

        do! fsc cfg "%s --target:exe -o:pos23.exe" cfg.fsc_flags ["pos23.fs"]
        do! peverify cfg "pos23.exe"
        do! exec cfg ("."/"pos23.exe") ""

        do! fsc cfg "%s --target:exe -o:pos20.exe" cfg.fsc_flags ["pos20.fs"]
        do! peverify cfg "pos20.exe"
        do! exec cfg ("."/"pos20.exe") ""

        do! fsc cfg "%s --target:exe -o:pos19.exe" cfg.fsc_flags ["pos19.fs"]
        do! peverify cfg "pos19.exe"
        do! exec cfg ("."/"pos19.exe") ""

        do! fsc cfg "%s --target:exe -o:pos18.exe" cfg.fsc_flags ["pos18.fs"]
        do! peverify cfg "pos18.exe"
        do! exec cfg ("."/"pos18.exe") ""

        do! fsc cfg "%s --target:exe -o:pos16.exe" cfg.fsc_flags ["pos16.fs"]
        do! peverify cfg "pos16.exe"
        do! exec cfg ("."/"pos16.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos17.exe  pos17.fs 
        do! fsc cfg "%s --target:exe -o:pos17.exe" cfg.fsc_flags ["pos17.fs"]
        // "%PEVERIFY%" pos17.exe
        do! peverify cfg "pos17.exe"
        // pos17.exe
        do! exec cfg ("."/"pos17.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos15.exe  pos15.fs 
        do! fsc cfg "%s --target:exe -o:pos15.exe" cfg.fsc_flags ["pos15.fs"]
        // "%PEVERIFY%" pos15.exe
        do! peverify cfg "pos15.exe"
        // pos15.exe
        do! exec cfg ("."/"pos15.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos14.exe  pos14.fs 
        do! fsc cfg "%s --target:exe -o:pos14.exe" cfg.fsc_flags ["pos14.fs"]
        // "%PEVERIFY%" pos14.exe
        do! peverify cfg "pos14.exe"
        // pos14.exe
        do! exec cfg ("."/"pos14.exe") ""

        // "%FSC%" %fsc_flags% --target:exe -o:pos13.exe  pos13.fs
        do! fsc cfg "%s --target:exe -o:pos13.exe" cfg.fsc_flags ["pos13.fs"]
        // "%PEVERIFY%" pos13.exe
        do! peverify cfg "pos13.exe"
        // pos13.exe
        do! exec cfg ("."/"pos13.exe") ""

        // "%FSC%" %fsc_flags% -a -o:pos12.dll  pos12.fs 
        do! fsc cfg "%s -a -o:pos12.dll" cfg.fsc_flags ["pos12.fs"]

        // "%FSC%" %fsc_flags% -a -o:pos11.dll  pos11.fs 
        do! fsc cfg "%s -a -o:pos11.dll" cfg.fsc_flags ["pos11.fs"]

        // "%FSC%" %fsc_flags% -a -o:pos10.dll  pos10.fs
        do! fsc cfg "%s -a -o:pos10.dll" cfg.fsc_flags ["pos10.fs"]

        // "%PEVERIFY%" pos10.dll
        do! peverify cfg "pos10.dll"

        // "%FSC%" %fsc_flags% -a -o:pos09.dll  pos09.fs
        do! fsc cfg "%s -a -o:pos09.dll" cfg.fsc_flags ["pos09.fs"]

        // "%PEVERIFY%" pos09.dll
        do! peverify cfg "pos09.dll"

        do! attempt.For (["neg97"; "neg96"; "neg95"; "neg94"; "neg93"; "neg92"; "neg91"; 
                          "neg90"; "neg89"; "neg88";
                          "neg87"; "neg86"; "neg85"; "neg84"; "neg83"; "neg82"; "neg81"; "neg80"; "neg79"; "neg78"; "neg77"; "neg76"; "neg75"; 
                          "neg74"; "neg73"; "neg72"; "neg71"; "neg70"; "neg69"; "neg68"; "neg67"; "neg66"; "neg65"; "neg64"; "neg61"; "neg63"; 
                          "neg62"; "neg20"; "neg24"; "neg32"; "neg37"; "neg37_a"; "neg60"; "neg59"; "neg58"; "neg57"; "neg56"; "neg56_a"; "neg56_b"; 
                          "neg55"; "neg54"; "neg53"; "neg52"; "neg51"; "neg50"; "neg49"; "neg48"; "neg47"; "neg46"; "neg10"; "neg10_a"; "neg45"; 
                          "neg44"; "neg43"; "neg38"; "neg39"; "neg40"; "neg41"; "neg42"], singleNegTest cfg)

        // "%FSC%" %fsc_flags% -a -o:pos07.dll  pos07.fs 
        do! fsc cfg "%s -a -o:pos07.dll" cfg.fsc_flags ["pos07.fs"]

        // "%PEVERIFY%" pos07.dll
        do! peverify cfg "pos07.dll"

        // "%FSC%" %fsc_flags% -a -o:pos08.dll  pos08.fs 
        do! fsc cfg "%s -a -o:pos08.dll" cfg.fsc_flags ["pos08.fs"]

        // "%PEVERIFY%" pos08.dll
        do! peverify cfg "pos08.dll"

        // "%FSC%" %fsc_flags% -a -o:pos06.dll  pos06.fs 
        do! fsc cfg "%s -a -o:pos06.dll" cfg.fsc_flags ["pos06.fs"]

        // "%PEVERIFY%" pos06.dll
        do! peverify cfg "pos06.dll"


        // "%FSC%" %fsc_flags% -a -o:pos03.dll  pos03.fs 
        do! fsc cfg "%s -a -o:pos03.dll" cfg.fsc_flags ["pos03.fs"]

        // "%PEVERIFY%" pos03.dll
        do! peverify cfg "pos03.dll"

        // "%FSC%" %fsc_flags% -a -o:pos03a.dll  pos03a.fsi pos03a.fs 
        do! fsc cfg "%s -a -o:pos03a.dll" cfg.fsc_flags ["pos03a.fsi"; "pos03a.fs"]

        // "%PEVERIFY%" pos03a.dll
        do! peverify cfg "pos03a.dll"

        do! attempt.For(["neg34"; "neg33"; "neg30"; "neg31"; "neg29"; "neg28"; "neg07"; "neg_byref_20"; 
                         "neg_byref_1"; "neg_byref_2"; "neg_byref_3"; "neg_byref_4"; "neg_byref_5"; "neg_byref_6"; "neg_byref_7"; "neg_byref_8"; 
                         "neg_byref_10"; "neg_byref_11"; "neg_byref_12"; "neg_byref_13"; "neg_byref_14"; "neg_byref_15"; "neg_byref_16"; 
                         "neg_byref_17"; "neg_byref_18"; "neg_byref_19"; "neg_byref_21"; "neg_byref_22"; "neg_byref_23"; "neg36"; "neg17"; "neg26"; 
                         "neg27"; "neg25"; "neg03"; "neg23"; "neg22"; "neg21"; "neg04"; "neg05"; "neg06"; "neg06_a"; "neg06_b"; "neg08"; "neg09"; 
                         "neg11"; "neg12"; "neg13"; "neg14"; "neg16"; "neg18"; "neg19"; "neg01"; "neg02"; "neg15" ], singleNegTest cfg)

        // echo Some random positive cases found while developing the negative tests
        // "%FSC%" %fsc_flags% -a -o:pos01a.dll  pos01a.fsi pos01a.fs
        do! fsc cfg "%s -a -o:pos01a.dll" cfg.fsc_flags ["pos01a.fsi"; "pos01a.fs"]

        // "%PEVERIFY%" pos01a.dll
        do! peverify cfg "pos01a.dll"

        // "%FSC%" %fsc_flags% -a -o:pos02.dll  pos02.fs
        do! fsc cfg "%s -a -o:pos02.dll" cfg.fsc_flags ["pos02.fs"]

        // "%PEVERIFY%" pos02.dll
        do! peverify cfg "pos02.dll"

        // call ..\..\single-neg-test.bat neg35
        do! singleNegTest cfg "neg35"

        // "%FSC%" %fsc_flags% -a -o:pos05.dll  pos05.fs
        do! fsc cfg "%s -a -o:pos05.dll" cfg.fsc_flags ["pos05.fs"]

        })
