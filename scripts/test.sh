TEST_PROJECTS = ( "OpenttdDiscord.Common.Tests" "OpenttdDiscord.Openttd.Tests" )

for i in "${TEST_PROJECTS[@]}"
do
   dotnet test $i
done