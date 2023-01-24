#!/bin/bash
dotnet ef migrations add $1 --startup-project ../NoteAPI --output-dir ../NoteAPI/Persistence/Migrations