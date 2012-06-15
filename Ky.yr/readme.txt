Ky.yr
=====

Knowit Reaktor Kyber - Umbraco Yr weather integration

This is a  Umbraco Package to integrate a server side XML integration with Yr.no
Formatting of the XML structure is controlled by a razor script and the Yr.no terms that you should cache data from their services for at least 10 minutes is adhered to by caching data for 15 minutes using a memorycache from System.Runtime.Caching.

Please note that you are required to display the Yr.no copyright notice and link to the yr.no page for the weatherdata that you display on your webpage.

Feel free to enhance this code.

Best regards
The Umbraco team in Knowit Reaktor Kyber

Package installs a macro you can use from your RTE or Templates and razor script to render output, a dll for caching data, the caching dll references the System.Runtime.Caching assembly for the 4.0 framework


Code for the DynamicXmlParser written by Kevin Hazzard - http://blogs.captechconsulting.com/blog/kevin-hazzard/fluent-xml-parsing-using-cs-dynamic-type-part-1