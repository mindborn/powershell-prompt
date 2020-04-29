
#region conda initialize
# !! Contents within this block are managed by 'conda init' !!
(& "C:\tools\miniconda3\Scripts\conda.exe" "shell.powershell" "hook") | Out-String | Invoke-Expression
#endregion

function prompt()
{
	$loc=Get-Location
	~\powershell-prompt.exe $loc.Path $loc.Provider.Name
}

[console]::InputEncoding = [console]::OutputEncoding = New-Object System.Text.UTF8Encoding


Get-PSDrive |  Where-Object {$_.Provider.Name -eq 'FileSystem'} | Format-Table @{Label="Drive"; Expression={$_.Name+"    "}}, @{Label="Free"; Expression={($_.Free/1GB).ToString("0.000").PadLeft(7)+" GB    "}}, @{Label="Used"; Expression={($_.Used/1GB).ToString("0.000").PadLeft(7)+" GB    "}} , @{Label="Total"; Expression={(($_.Free+$_.Used)/1GB).ToString("0.000").PadLeft(7)+" GB    "}} -AutoSize
