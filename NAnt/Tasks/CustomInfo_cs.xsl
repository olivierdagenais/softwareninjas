<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
    <xsl:output method="text" encoding="UTF-8" indent="yes" />

    <xsl:param name="buildNumber" />
    <xsl:param name="registeredUserDisplayName" />
    <xsl:param name="registeredUserEmailAddress" />

    <xsl:template match="/version">
        <xsl:variable name="version" select="concat(@major, '.', @minor, '.', $buildNumber)" />

<xsl:text />using System.Reflection;
using SoftwareNinjas.Core;

<xsl:if test="string-length($registeredUserDisplayName) &gt; 0 and string-length($registeredUserEmailAddress) &gt; 0">
[assembly: RegisteredUser ( "<xsl:value-of select="$registeredUserDisplayName"/>", "<xsl:value-of select="$registeredUserEmailAddress"/>" )]
</xsl:if>

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion ( "<xsl:value-of select="$version"/>" )]
[assembly: AssemblyFileVersion ( "<xsl:value-of select="$version"/>" )]
</xsl:template>

</xsl:stylesheet>
