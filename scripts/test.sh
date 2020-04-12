# TEST_PROJECTS = ( "OpenttdDiscord.Common.Tests" "OpenttdDiscord.Openttd.Tests" )
TEST_PROJECTS[0] = "OpenttdDiscord.Common.Tests"
TEST_PROJECTS[1] = "OpenttdDiscord.Openttd.Tests"

for i in "${TEST_PROJECTS[@]}"
do
   dotnet test $i
done