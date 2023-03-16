
Following are the deployment instructions:

Prior to executing the terraform script login to azure using az cli using an account which has permissions to provision azure services in the targeted subscription. 

1.	Run the TF Script. This will accomplish the following:
    a.	Provision the Speech service to translate speech to text.
    b.	Provision the Azure Function to host the Broadcast function.
    c.	Provision the Azure Web App to host the web application.
    d.	Provision the storage account to host the code for the azure function and web app. 
2.	Make a note the speech app key from the above created speech service. 
3.	Code update:
    a.	BroadcastFunction/Utils/Constants.cs : Update the list of languages.
    b.	Speech-app/src/lib/constancts.js : Update the key apiBaseUrl to the base uri of the azure functions.
4.	Deploy code:
    a.	BroadcastFunction to the Azure Function created above.
    b.	Speech-app to the Azure web app created above.
