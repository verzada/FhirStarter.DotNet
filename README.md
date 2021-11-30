# FhirStarter.DotNetCore3.0
FhirStarter based on .Net Core 3.0.

This version of FhirStarter supports two different versions of FHIR.

* STU3
* R4 

The code behind the libraries are pretty much the same, but are based on two different libraries. 

## Getting started - STU3 
[Getting started with STU3](src/docs/STU3/GettingStarted-STU3.md)


## Getting started - R4

[Getting started with R4](src/docs/R4/GettingStarted-R4.md)

## Note 

To override output from headers use

|Type of header|Value|Necessary|
|-|-|-|
|Accept|"application/json+fhir"|Yes, will not override without it|
|Accept|"application/xml+fhir"|Yes, will not override without it|
|Accept-Content|Does not work|Will return the first type of medeatype that is set in the startup|
|Content-Type|"application/json+fhir"|To be compliant|
|Content-Type|"application/xml+fhir"|To be compliant|


