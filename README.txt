Setup Instructions

1. Install Module in Sitecore
2. Modify web.config using the instructions below:

If you are using IIS 7 and over, add the following to your web.config in the system.webServer, modules section:
<system.webServer>
	<modules>
	...
		<add name="NTT.Minifier" type="NTT.Minifier.HTMLMinifier, NTT.Minifier"/>
	</modules>
</system.webServer>

If you are using IIS version lower than 7, add the following to your web.config in the system.web, modules section:
<system.web>
	<httpModules>
		<add name="NTT.Minifier" type="NTT.Minifier.HTMLMinifier, NTT.Minifier"/>
	</httpModules>
</system.web>