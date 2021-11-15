param ([Parameter(Mandatory=$true)] [String] $profilingInputsPath, [String] $bootstrapperInfoPath, [String] $buildDropPath)

Write-Host 'Inputs:'
Write-Host "profilingInputsPath: $profilingInputsPath"
Write-Host "bootstrapperInfoPath: $bootstrapperInfoPath"
Write-Host "buildDropPath: $buildDropPath"

$runsettingsPath = (Get-Item "$PSScriptRoot\OptProf.runsettings").FullName
$runsettingsXml = [Xml.XmlDocument](Get-Content $runsettingsPath)
# https://stackoverflow.com/questions/33813700/empty-xml-node-rendered-as-string-in-powershell
$testStores = $runsettingsXml.RunSettings.TestConfiguration.SelectSingleNode('TestStores')

$profilingInputsStore = $runsettingsXml.CreateElement('TestStore')
$profilingInputsStore.SetAttribute('Uri', $profilingInputsPath)
$testStores.AppendChild($profilingInputsStore)

$buildDropStore = $runsettingsXml.CreateElement('TestStore')
if(-not $buildDropPath)
{
  if((-not $bootstrapperInfoPath) -or (-not (Test-Path $bootstrapperInfoPath)))
  {
    Write-Host "Invalid bootstrapperInfoPath: $bootstrapperInfoPath"
    exit -1
  }
  $buildDropJson = Get-Content $bootstrapperInfoPath | ConvertFrom-Json
  $dropHashAndGuid = $buildDropJson[0].BuildDrop.TrimStart('https://vsdrop.corp.microsoft.com/file/v1/Products/DevDiv/VS/')
  $buildDropPath = "Tests/DevDiv/VS/$dropHashAndGuid"
}
$buildDropStore.SetAttribute('Uri', $buildDropPath)
$testStores.AppendChild($buildDropStore)

$runsettingsXml.Save($runsettingsPath)
Write-Host 'Saved Output:'
Write-Host "profilingInputsStore: $profilingInputsStore"
Write-Host "buildDropStore: $buildDropStore"