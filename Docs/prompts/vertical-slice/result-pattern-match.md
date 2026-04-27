<task>I want to modify the current plan for vertical slice refactor to standartize the Endpoint return on entire Api.</task>

<role>As a software Architect you are responsible to adjust a existend plan files that is related to an refactor this entire application.</role>

<requirements>
 - Analyze all markdown files at ./Docs/Tasks/prd-vertical-slice-refactor folder
 - Add to the corret tasks files, prd file, techspech file the new requirement.
 - The new requirement is to standartize the returns of Api returning the Result pattern object thet each handler class will return.
 - Look a the example <example> to take the idea of implementation.
 - The http verbs that returns values, ***produced by endpoints*** like OK, BadRequest, Notfound, etc. Should return the result object pattern that handler will return.
 - The http verbs that not return values hould stay returng nothing as content, like "NoContent".
 - Even handler return a Result object withouT the "value" it should be returned by endpoint. The type "Result<T>" will have an value inside it in the property "Value"   
</requirements>

<example>

### This is a conceitual example of the idea of implementation

app.MapPost(
"users/{userId}/follow/{followedId}",
(Guid userId, Guid followedId, FollowerService followerService) =>
{
var result = await followerService.StartFollowingAsync(
userId,
followedId,
DateTime.UtcNow);

        if (result.IsFailure)
        {
            return Results.BadRequest(result);
        }

        return Results.OK(result);
    });

</example>

<skills> 
  - Use the result-pattern skill to implement the result pattern.
  - Use the minimal-api skill to implement the endpoints register and mappign configuration
</skills>

<critical>
    ### Mandatory Skills

    - result-pattern — to implement the result pattern
    - minimal-api skill — to implement the endpoints register and mappign configuration

    ### Ask if needed
    USE ASK USER QUESTION TOOL if somenthing is not clear or any information missing is detected.

</critical>

<references>

### All Files at this folder:

./Docs/Tasks/prd-vertical-slice-refactor/

</references>
