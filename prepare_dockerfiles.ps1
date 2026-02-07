# This file is used to copy all .csproj files present in solution during dockerfile build in order to be able to dotnet restore them 
# so Docker is able to cache nuget package download process.

# If you add/remove project from solution you need to run this file in order to include changes in dockerfiles.
function Prepare-Dockerfile($dockerfile)
{
    Write-Host $project
    Write-Host $dockerfile
    $projects = Get-ChildItem ./**/*.csproj 

    $output = "";
    $fetching_projects = $false;
    $outdockerfile = "./${dockerfile}"

    Write-Host $outdockerfile

    foreach($line in Get-Content -Encoding UTF8 $outdockerfile) {
        if($line -match "#PUT_PROJECTS_BELOW_THIS_LINE"){ 
            $output += "#PUT_PROJECTS_BELOW_THIS_LINE`n"
            $fetching_projects = $true;
            $propsFiles = @("Directory.Build.props", "Directory.Packages.props")
            foreach($propsFile in $propsFiles)
            {
                if(Test-Path "./$propsFile")
                {
                    $output += "COPY ./${propsFile} ./${propsFile}`n"
                }
            }
            foreach($proj in $projects)
            {
                $output += "COPY "
                $output += ($proj.FullName | Resolve-Path -Relative).Replace("\", "/")
                $output += " "
                $output += ($proj.FullName | Resolve-Path -Relative).Replace("\", "/")
                $output += "`n"
            }
        } elseif($line -match "#END_PUT_PROJECTS_BELOW_THIS_LINE") {
            $output += "#END_PUT_PROJECTS_BELOW_THIS_LINE`n"
            $fetching_projects = $false
        } elseif($fetching_projects -match $false) {
            $output += $line;
            $output += "`n";
            
        }
    }
    $output | Out-File -Encoding UTF8 $outdockerfile
}


Prepare-Dockerfile "Dockerfile"