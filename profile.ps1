
#region conda initialize
# !! Contents within this block are managed by 'conda init' !!
(& "C:\tools\Anaconda3\Scripts\conda.exe" "shell.powershell" "hook") | Out-String | Invoke-Expression
#endregion

function prompt()
{
	$loc=Get-Location
	powershell-prompt.exe $loc.Path $loc.Provider.Name
}

[console]::InputEncoding = [console]::OutputEncoding = New-Object System.Text.UTF8Encoding
