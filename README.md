# das-findtraineeship-api

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-findtraineeship-api?repoName=SkillsFundingAgency%2Fdas-findtraineeship-api&branchName=FAI-298-Get_Traineeships_for_the_Display_API)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status/das-findtraineeship-api?repoName=SkillsFundingAgency%2Fdas-findtraineeship-api&branchName=FAI-298-Get_Traineeships_for_the_Display_API)


The das-findapprenticeship-api is the inner api for retrieving and filtering traineeship vacancies relying on the ElasticIndex created from das-findapprenticeship.

## How It Works

### Requirements
• DotNet Core 3.1 and any supported IDE for DEV running.
• Azure Storage Account
• ElasticIndex created from das-findapprenticeship

### Configuration
• In your Azure Storage Account, create a table called Configuration and Add the following
```
ParitionKey: LOCAL
RowKey: SFA.DAS.FindTraineeships.Api_1.0
Data: 
{
  "FindTraineeshipsApi": {
    "ElasticSearchUsername": " ",
    "ElasticSearchPassword": " ",
    "ElasticSearchServerUrl": " "
  },
}
```
 