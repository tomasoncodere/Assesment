# Assesment

## Notes on Decisions
* I used MongoDB because I didn't see the need for additional tables. There are no queries based on producers, etc.
* I didn't map all the fields because there were quite a few, and I believe they don't add anything to the assessment.
* I used Aspire as the orchestrator because I find it much more intuitive and better integrated with the IDE compared to Docker Compose.
* I know I didn’t use mappers to convert DTOs to entities, which is a bad practice. This was done to shorten the implementation time for the assessment.
* I added unit tests and integration tests to the parts I consider the most important in the project.
* I understand that batching during the synchronization of shows would have been a valuable improvement. However, with Chunk, this doesn't present any technical challenge, and I wanted to keep the assessment shorter.
* The Migrations program is simply to seed the database with some test keys

## Prerequisites
* Docker
* .net Aspire

## To run it from Visual Studio 2022
If you match the prerequisites, just press F5

