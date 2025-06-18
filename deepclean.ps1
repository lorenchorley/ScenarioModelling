$root = Get-Location

$projectDirs = Get-ChildItem -Path $root -Recurse -Filter *.csproj | ForEach-Object { $_.Directory.FullName } | Sort-Object -Unique

foreach ($dir in $projectDirs) {
	Write-Host ""
    Write-Host "Project folder detected : $dir"

    $binPath = Join-Path -Path $dir -ChildPath "bin"
    $objPath = Join-Path -Path $dir -ChildPath "obj"

    if (Test-Path $binPath) {
        try {
            Write-Host "Deleting : $binPath"
			Remove-Item -Path $binPath -Recurse -Force -ErrorAction Stop
        } catch {
			Write-Warning "Delete failure ${binPath}: $_"
        }
    }

    if (Test-Path $objPath) {
        try {
            Write-Host "Deleting : $objPath"
			Remove-Item -Path $objPath -Recurse -Force -ErrorAction Stop
        } catch {
            Write-Warning "Delete failure ${objPath}: $_"
        }
    }
}

$vsDir = ".vs"
if (Test-Path -Path $vsDir) {
    Write-Host "Deleting : $vsDir"
	
	try {
		Remove-Item -Recurse -Force $vsDir
	} catch {
		Write-Warning "Delete failure ${vsDir}: $_"
	}
}
