param(
    [string]$RunFolder = "Runs\agent-full-20260611-fresh5",
    [string]$FfmpegPath = "",
    [string]$Out = "Recordings\adventureworks-meta-bi-demo-black-white.mp4"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$runPath = Join-Path $root $RunFolder
if (-not (Test-Path $runPath)) {
    throw "Run folder not found: $runPath"
}

if ([string]::IsNullOrWhiteSpace($FfmpegPath)) {
    $repoRoot = Resolve-Path (Join-Path $root "..\..\..")
    $ffmpeg = Get-ChildItem -Path (Join-Path $repoRoot "artifacts\tools\ffmpeg") -Filter ffmpeg.exe -Recurse -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
    if (-not $ffmpeg) {
        $ffmpeg = (Get-Command ffmpeg -ErrorAction SilentlyContinue).Source
    }
    $FfmpegPath = $ffmpeg
}

if ([string]::IsNullOrWhiteSpace($FfmpegPath) -or -not (Test-Path $FfmpegPath)) {
    throw "ffmpeg.exe was not found. Pass -FfmpegPath or install/download ffmpeg."
}

$outPath = if ([System.IO.Path]::IsPathRooted($Out)) { $Out } else { Join-Path $root $Out }
$outDir = Split-Path -Parent $outPath
$workDir = Join-Path $outDir "black-white-video-frames"
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
if (Test-Path $workDir) {
    Remove-Item -LiteralPath $workDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $workDir | Out-Null

Add-Type -AssemblyName System.Drawing

$script:FrameIndex = 1
$script:ConcatEntries = New-Object System.Collections.Generic.List[string]

function New-Brush($hex) {
    return [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($hex))
}

function New-Pen($hex, $width = 1) {
    return [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($hex), $width)
}

function New-Font($family, $size, $style = [System.Drawing.FontStyle]::Regular) {
    return [System.Drawing.Font]::new($family, [float]$size, $style, [System.Drawing.GraphicsUnit]::Pixel)
}

function Draw-Text($graphics, $text, $font, $brush, $x, $y, $w, $h, $near = $true) {
    $format = [System.Drawing.StringFormat]::new()
    $format.Trimming = [System.Drawing.StringTrimming]::EllipsisWord
    if (-not $near) {
        $format.Alignment = [System.Drawing.StringAlignment]::Center
        $format.LineAlignment = [System.Drawing.StringAlignment]::Center
    }
    $rect = [System.Drawing.RectangleF]::new([float]$x, [float]$y, [float]$w, [float]$h)
    $graphics.DrawString($text, $font, $brush, $rect, $format)
    $format.Dispose()
}

function Add-Frame($bitmap, $duration) {
    $file = Join-Path $workDir ("frame{0:000}.png" -f $script:FrameIndex)
    $bitmap.Save($file, [System.Drawing.Imaging.ImageFormat]::Png)
    $script:ConcatEntries.Add(("file '{0}'" -f ($file -replace "\\", "/"))) | Out-Null
    $script:ConcatEntries.Add(("duration {0}" -f $duration)) | Out-Null
    $script:FrameIndex++
}

function New-Canvas {
    $bmp = [System.Drawing.Bitmap]::new(1920, 1080)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::ClearTypeGridFit
    $bg = New-Brush "#050505"
    $g.FillRectangle($bg, 0, 0, 1920, 1080)
    $bg.Dispose()
    return [pscustomobject]@{ Bitmap = $bmp; Graphics = $g }
}

function Close-Canvas($canvas) {
    $canvas.Graphics.Dispose()
    $canvas.Bitmap.Dispose()
}

function Draw-Header($g) {
    $white = New-Brush "#f4f4f4"
    $line = New-Pen "#242424" 2
    Draw-Text $g "meta + meta-bi" (New-Font "Segoe UI Semibold" 32) $white 92 34 500 50
    $g.DrawLine($line, 90, 102, 1830, 102)
    $white.Dispose()
    $line.Dispose()
}

function Render-TitleSlide($title, $subtitle, $duration) {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    $white = New-Brush "#f8f8f8"
    $muted = New-Brush "#b8b8b8"
    $line = New-Pen "#f8f8f8" 4
    Draw-Text $g $title (New-Font "Segoe UI Black" 86) $white 150 390 1620 110 $false
    if (-not [string]::IsNullOrWhiteSpace($subtitle)) {
        Draw-Text $g $subtitle (New-Font "Segoe UI" 32) $muted 240 515 1440 60 $false
    }
    $g.DrawLine($line, 610, 615, 1310, 615)
    Add-Frame $canvas.Bitmap $duration
    $line.Dispose()
    $white.Dispose()
    $muted.Dispose()
    Close-Canvas $canvas
}

function Render-CenteredStatement($title, $body, $duration) {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    Draw-Header $g
    $white = New-Brush "#f8f8f8"
    $muted = New-Brush "#bcbcbc"
    Draw-Text $g $title (New-Font "Segoe UI Black" 76) $white 150 335 1620 110 $false
    Draw-Text $g $body (New-Font "Segoe UI" 34) $muted 285 480 1350 90 $false
    Add-Frame $canvas.Bitmap $duration
    $white.Dispose()
    $muted.Dispose()
    Close-Canvas $canvas
}

function Wrap-Line($line, $maxChars) {
    if ($line.Length -le $maxChars) { return @($line) }
    $words = $line -split " "
    $result = New-Object System.Collections.Generic.List[string]
    $current = ""
    foreach ($word in $words) {
        $candidate = if ($current.Length -eq 0) { $word } else { "$current $word" }
        if ($candidate.Length -gt $maxChars -and $current.Length -gt 0) {
            $result.Add($current) | Out-Null
            $current = $word
        }
        else {
            $current = $candidate
        }
    }
    if ($current.Length -gt 0) { $result.Add($current) | Out-Null }
    return $result.ToArray()
}

function Get-DocumentLines($path, $maxChars) {
    $rawLines = (Get-Content $path) -replace "`t", "    "
    $lines = New-Object System.Collections.Generic.List[string]
    foreach ($line in $rawLines) {
        if ([string]::IsNullOrWhiteSpace($line)) {
            $lines.Add("") | Out-Null
            continue
        }
        foreach ($wrapped in Wrap-Line $line $maxChars) {
            $lines.Add($wrapped) | Out-Null
        }
    }
    return $lines.ToArray()
}

function Render-DocumentFrame($heading, $displayPath, $lines, $start, $duration) {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    Draw-Header $g
    $white = New-Brush "#f5f5f5"
    $muted = New-Brush "#9f9f9f"
    $ink = New-Brush "#e6e6e6"
    $panelBrush = New-Brush "#111111"
    $linePen = New-Pen "#303030" 2
    Draw-Text $g $heading (New-Font "Segoe UI Black" 52) $white 130 145 1500 70
    Draw-Text $g $displayPath (New-Font "Consolas" 23) $muted 130 220 1500 40
    $g.FillRectangle($panelBrush, 130, 285, 1660, 665)
    $g.DrawRectangle($linePen, 130, 285, 1660, 665)
    $font = New-Font "Consolas" 22
    $y = 325
    $top = [Math]::Floor($start)
    $offset = $start - $top
    $visibleLineCount = 22
    for ($i = $top; $i -lt [Math]::Min($top + $visibleLineCount, $lines.Count); $i++) {
        Draw-Text $g $lines[$i] $font $ink 170 ($y - [int](28 * $offset)) 1580 28
        $y += 28
    }
    $track = New-Pen "#303030" 4
    $thumb = New-Pen "#f5f5f5" 5
    $g.DrawLine($track, 1810, 285, 1810, 950)
    $maxStart = [Math]::Max($lines.Count - $visibleLineCount, 1)
    $ratio = [Math]::Min([Math]::Max($start / $maxStart, 0), 1)
    $thumbTop = 285 + [int]((950 - 285 - 95) * $ratio)
    $g.DrawLine($thumb, 1810, $thumbTop, 1810, $thumbTop + 95)
    Add-Frame $canvas.Bitmap $duration
    $white.Dispose(); $muted.Dispose(); $ink.Dispose(); $panelBrush.Dispose(); $linePen.Dispose(); $track.Dispose(); $thumb.Dispose()
    Close-Canvas $canvas
}

function Render-DocumentScroll($heading, $path, $durationSeconds) {
    $lines = Get-DocumentLines $path 96
    $displayPath = [System.IO.Path]::GetFileName($path)
    $frameRate = 10
    $frameCount = [Math]::Max([int]($durationSeconds * $frameRate), 1)
    $maxStart = [Math]::Max($lines.Count - 22, 0)
    for ($frame = 0; $frame -lt $frameCount; $frame++) {
        $ratio = if ($frameCount -eq 1) { 0 } else { $frame / ($frameCount - 1) }
        $start = $maxStart * $ratio
        Render-DocumentFrame $heading $displayPath $lines $start (1 / $frameRate)
    }
}

function Render-TerminalFrame($heading, $subheading, $lines, $start, $visibleCount, $duration) {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    Draw-Header $g
    $white = New-Brush "#f5f5f5"
    $muted = New-Brush "#aaaaaa"
    $ink = New-Brush "#e8e8e8"
    $prompt = New-Brush "#ffffff"
    $panel = New-Brush "#0d0d0d"
    $linePen = New-Pen "#3a3a3a" 2
    Draw-Text $g $heading (New-Font "Segoe UI Black" 52) $white 130 145 1500 70
    Draw-Text $g $subheading (New-Font "Segoe UI" 25) $muted 130 220 1500 40
    $g.FillRectangle($panel, 130, 285, 1660, 665)
    $g.DrawRectangle($linePen, 130, 285, 1660, 665)
    $font = New-Font "Consolas" 23
    $y = 325
    $top = [Math]::Floor($start)
    $offset = $start - $top
    for ($i = $top; $i -lt [Math]::Min($top + $visibleCount, $lines.Count); $i++) {
        $line = $lines[$i]
        $brush = if ($line.StartsWith(">")) { $prompt } else { $ink }
        Draw-Text $g $line $font $brush 170 ($y - [int](32 * $offset)) 1580 31
        $y += 32
        if ($y -gt 910) { break }
    }
    Add-Frame $canvas.Bitmap $duration
    $white.Dispose(); $muted.Dispose(); $ink.Dispose(); $prompt.Dispose(); $panel.Dispose(); $linePen.Dispose()
    Close-Canvas $canvas
}

function Render-Terminal($heading, $subheading, $lines, $duration) {
    Render-TerminalFrame $heading $subheading $lines 0 18 $duration
}

function Render-TerminalReveal($heading, $subheading, $lines, $durationSeconds) {
    $frameRate = 8
    $frameCount = [Math]::Max([int]($durationSeconds * $frameRate), 1)
    $visibleCount = 18
    for ($frame = 0; $frame -lt $frameCount; $frame++) {
        $lineCount = [Math]::Min($lines.Count, [Math]::Max(1, [int][Math]::Ceiling(($frame + 1) / $frameCount * $lines.Count)))
        $window = $lines[0..($lineCount - 1)]
        $start = [Math]::Max(0, $lineCount - $visibleCount)
        Render-TerminalFrame $heading $subheading $window $start $visibleCount (1 / $frameRate)
    }
}

function Render-Parts($duration) {
    $canvas = New-Canvas
    $g = $canvas.Graphics
    Draw-Header $g
    $white = New-Brush "#f5f5f5"
    $muted = New-Brush "#bdbdbd"
    $panel = New-Brush "#111111"
    $linePen = New-Pen "#3a3a3a" 2
    Draw-Text $g "Generated parts" (New-Font "Segoe UI Black" 62) $white 130 145 1500 78
    $items = @(
        @("PLAN.md", "phased run plan with review gates"),
        @("source\AdventureWorks2022\Schema", "live OLTP source contract"),
        @("rdv\...\RawVault", "source-grain historized evidence"),
        @("bdv\...\BusinessVault", "business keys and integrated relationships"),
        @("dw\...\Warehouse + Transforms", "analytical delivery layer"),
        @("dw\...\Binding + Quality", "validated transforms and model-derived DQ"),
        @("ops\Pipeline + Orchestration", "inferred run plan and modeled execution")
    )
    $y = 285
    foreach ($item in $items) {
        $g.FillRectangle($panel, 160, $y, 1600, 75)
        $g.DrawRectangle($linePen, 160, $y, 1600, 75)
        Draw-Text $g $item[0] (New-Font "Consolas" 27) $white 195 ($y + 20) 540 36
        Draw-Text $g $item[1] (New-Font "Segoe UI" 25) $muted 765 ($y + 20) 900 36
        $y += 88
    }
    Add-Frame $canvas.Bitmap $duration
    $white.Dispose(); $muted.Dispose(); $panel.Dispose(); $linePen.Dispose()
    Close-Canvas $canvas
}

$businessPath = Join-Path $root "BUSINESS-REQUIREMENTS.md"
$guidePath = Join-Path $root "agent-meta.md"

Render-TitleSlide "meta + meta-bi demo" "2026-Jun" 4
Render-CenteredStatement "Generate a full BI stack" "from one business requirements document, in phases" 4
Render-DocumentScroll "One Business Request" $businessPath 10
Render-DocumentScroll "One Agent Guide" $guidePath 12
Render-TerminalReveal "Prompt The Worker" "The worker receives the brief, the generic guide, and connection variables." @(
    "> worker-agent",
    "Read BUSINESS-REQUIREMENTS.md.",
    "Read agent-meta.md.",
    "Use AW_SOURCE_SQL, AW_TARGET_SQL, AW_TABULAR_SERVER.",
    "Create a clean run folder.",
    "Write PLAN.md before product commands.",
    "Generate visible .cmd stage scripts.",
    "Build SourceDB -> RDV -> BDV -> DW/Mart -> Tabular.",
    "Stop and record blockers instead of pretending.",
    "",
    "Expected shape:",
    "SourceDB -> RDV -> BDV -> DW/Mart -> Tabular",
    "with transforms, binding, model-derived DQ, pipeline, and inferred orchestration."
) 7
Render-TerminalReveal "The Agent Builds" "Phased run with layer gates." @(
    "> run.cmd",
    "Running AdventureWorks BI stack demo in phases...",
    "> type PLAN.md",
    "Phases: source, RDV, BDV, DW, binding/DQ, analytics, orchestration",
    "> meta-schema extract sqlserver --system AdventureWorks2022",
    "Source inspection: SalesOrderDetail 121,317 rows; Product 504 rows",
    "> meta-convert schema-to-raw-datavault",
    "RDV gate: raw vault model preserves source-grain evidence",
    "> meta-datavault-business help",
    "BDV gate: business vault model or named product gap",
    "> meta-transform-script from sql-files --manifest transform-manifest.tsv",
    "DW gate: mart transforms imported into dw\\...\\Transforms",
    "> meta-sql deploy-plan / meta-sql deploy",
    "Deployment gate: layer SQL assets planned or deployed",
    "> meta-transform-binding bind",
    "Binding gate: strict validation before DQ and orchestration"
) 10
Render-TerminalReveal "Replayable Commands" "The generated replay script keeps the run visible and repeatable." @(
    "> type run.cmd",
    "call stages\\00-source-readiness.cmd",
    "call stages\\01-extract-source-schema.cmd",
    "call stages\\02-author-rdv.cmd",
    "call stages\\03-author-bdv.cmd",
    "call stages\\04-author-dw-mart.cmd",
    "call stages\\05-bind-layer-transforms.cmd",
    "call stages\\06-data-quality.cmd",
    "call stages\\07-realize-or-deploy-sql.cmd",
    "call stages\\08-author-analytics.cmd",
    "call stages\\09-author-pipeline.cmd",
    "call stages\\10-author-orchestration.cmd",
    "call stages\\11-execute-orchestration.cmd",
    "",
    "Completed configured stages."
) 8
Render-Parts 9
Render-TerminalReveal "Tabular Proof" "SSMS / DAX proof against .\\TABULAR / AdventureWorksMetaDemoTabular." @(
    "> EVALUATE ROW(",
    "    `"FactRows`", COUNTROWS('Fact Sales Order Line'),",
    "    `"ProductRows`", COUNTROWS('Product'),",
    "    `"QuotaRows`", COUNTROWS('Salesperson Quota')",
    "  )",
    "",
    "FactRows     121317",
    "ProductRows  504",
    "QuotaRows    163",
    "",
    "SQL mart and Tabular counts match."
) 9
Render-TitleSlide "BI stack generated" "modeled, deployed, validated, processed" 4

$lastFrame = Join-Path $workDir ("frame{0:000}.png" -f ($script:FrameIndex - 1))
$script:ConcatEntries.Add(("file '{0}'" -f ($lastFrame -replace "\\", "/"))) | Out-Null
$concat = Join-Path $workDir "frames.txt"
$script:ConcatEntries | Set-Content -Path $concat -Encoding ASCII

& $FfmpegPath -y -f concat -safe 0 -i $concat -vf "fps=30,format=yuv420p" -movflags +faststart $outPath
if ($LASTEXITCODE -ne 0) {
    throw "ffmpeg failed with exit code $LASTEXITCODE"
}

Write-Host "Video written to $outPath"
