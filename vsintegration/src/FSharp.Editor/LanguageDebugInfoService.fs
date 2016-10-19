﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Linq

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Debugging
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<Shared>]
[<ExportLanguageService(typeof<ILanguageDebugInfoService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpLanguageDebugInfoService() =

    static member GetDataTipInformation(sourceText: SourceText, position: int, tokens: List<ClassifiedSpan>): TextSpan option =
        let tokenIndex = tokens |> Seq.tryFindIndex(fun t -> t.TextSpan.Contains(position))

        if tokenIndex.IsNone then
            None
        else
            let token = tokens.[tokenIndex.Value]
        
            match token.ClassificationType with

            | ClassificationTypeNames.StringLiteral ->
                Some(token.TextSpan)

            | ClassificationTypeNames.Identifier ->
                let textLine = sourceText.Lines.GetLineFromPosition(position)
                match QuickParse.GetCompleteIdentifierIsland false (textLine.ToString()) (position - textLine.Start) with
                | None -> None
                | Some(island, islandEnd, _) ->
                    let islandDocumentStart = textLine.Start + islandEnd - island.Length
                    Some(TextSpan.FromBounds(islandDocumentStart, islandDocumentStart + island.Length))

            | _ -> None


    interface ILanguageDebugInfoService with
        
        // FSROSLYNTODO: This is used to get function names in breakpoint window. It should return fully qualified function name and line offset from the start of the function.
        member this.GetLocationInfoAsync(_, _, _): Task<DebugLocationInfo> =
            Task.FromResult(Unchecked.defaultof<DebugLocationInfo>)

        member this.GetDataTipInfoAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<DebugDataTipInfo> =
            let computation = async {
                match FSharpLanguageService.GetOptions(document.Project.Id) with
                | Some(options) ->
                    let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let textSpan = TextSpan.FromBounds(0, sourceText.Length)
                    let tokens = FSharpColorizationService.GetColorizationData(sourceText, textSpan, Some(document.Name), defines, cancellationToken)
                    return match FSharpLanguageDebugInfoService.GetDataTipInformation(sourceText, position, tokens) with
                           | None -> Unchecked.defaultof<DebugDataTipInfo>
                           | Some(textSpan) -> new DebugDataTipInfo(textSpan, sourceText.GetSubText(textSpan).ToString())
                | None -> return Unchecked.defaultof<DebugDataTipInfo>
            }
            
            Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
                 .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)
            
            