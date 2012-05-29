<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="FTP2Azure" generation="1" functional="0" release="0" Id="1eb76584-111c-4aab-a968-1c7e6a6685b1" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="FTP2AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="FTPServerRole:bentchmark" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/FTP2Azure/FTP2AzureGroup/LB:FTPServerRole:bentchmark" />
          </inToChannel>
        </inPort>
        <inPort name="FTPServerRole:FTP" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/FTP2Azure/FTP2AzureGroup/LB:FTPServerRole:FTP" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="FTPServerRole:AccountKey" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:AccountKey" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:AccountName" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:AccountName" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:BaseUri" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:BaseUri" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:DiagnosticsConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:DiagnosticsConnectionString" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:Mode" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:Mode" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:ProviderName" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:ProviderName" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:UseAsyncMethods" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:UseAsyncMethods" />
          </maps>
        </aCS>
        <aCS name="FTPServerRole:UseHttps" defaultValue="">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRole:UseHttps" />
          </maps>
        </aCS>
        <aCS name="FTPServerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/FTP2Azure/FTP2AzureGroup/MapFTPServerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:FTPServerRole:bentchmark">
          <toPorts>
            <inPortMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/bentchmark" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:FTPServerRole:FTP">
          <toPorts>
            <inPortMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/FTP" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapFTPServerRole:AccountKey" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/AccountKey" />
          </setting>
        </map>
        <map name="MapFTPServerRole:AccountName" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/AccountName" />
          </setting>
        </map>
        <map name="MapFTPServerRole:BaseUri" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/BaseUri" />
          </setting>
        </map>
        <map name="MapFTPServerRole:DiagnosticsConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/DiagnosticsConnectionString" />
          </setting>
        </map>
        <map name="MapFTPServerRole:Mode" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/Mode" />
          </setting>
        </map>
        <map name="MapFTPServerRole:ProviderName" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/ProviderName" />
          </setting>
        </map>
        <map name="MapFTPServerRole:UseAsyncMethods" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/UseAsyncMethods" />
          </setting>
        </map>
        <map name="MapFTPServerRole:UseHttps" kind="Identity">
          <setting>
            <aCSMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole/UseHttps" />
          </setting>
        </map>
        <map name="MapFTPServerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="FTPServerRole" generation="1" functional="0" release="0" software="C:\Users\Chine\Documents\Visual Studio 2010\Projects\AzureFTP\FTP2Azure\csx\Release\roles\FTPServerRole" entryPoint="base\x86\WaHostBootstrapper.exe" parameters="base\x86\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="bentchmark" protocol="http" portRanges="80" />
              <inPort name="FTP" protocol="tcp" portRanges="21" />
            </componentports>
            <settings>
              <aCS name="AccountKey" defaultValue="" />
              <aCS name="AccountName" defaultValue="" />
              <aCS name="BaseUri" defaultValue="" />
              <aCS name="DiagnosticsConnectionString" defaultValue="" />
              <aCS name="Mode" defaultValue="" />
              <aCS name="ProviderName" defaultValue="" />
              <aCS name="UseAsyncMethods" defaultValue="" />
              <aCS name="UseHttps" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FTPServerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FTPServerRole&quot;&gt;&lt;e name=&quot;bentchmark&quot; /&gt;&lt;e name=&quot;FTP&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRoleInstances" />
            <sCSPolicyFaultDomainMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyFaultDomain name="FTPServerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FTPServerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="6f07567e-0bdc-4f86-a374-a71da2b5bff9" ref="Microsoft.RedDog.Contract\ServiceContract\FTP2AzureContract@ServiceDefinition.build">
      <interfacereferences>
        <interfaceReference Id="9d020873-71e0-4849-9dcc-5375423f2e07" ref="Microsoft.RedDog.Contract\Interface\FTPServerRole:bentchmark@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole:bentchmark" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="846eac16-e602-498d-a62a-f8b86e83b94f" ref="Microsoft.RedDog.Contract\Interface\FTPServerRole:FTP@ServiceDefinition.build">
          <inPort>
            <inPortMoniker name="/FTP2Azure/FTP2AzureGroup/FTPServerRole:FTP" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>