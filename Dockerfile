FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /sln
COPY projectfiles.tar .
RUN tar -xvf projectfiles.tar && dotnet restore Highstreetly.sln
COPY ./src ./src 
RUN dotnet build Highstreetly.sln -c Release --no-restore --verbosity minimal
#RUN dotnet test --no-restore --verbosity minimal
RUN dotnet publish Highstreetly.sln -c Release -o out --no-restore --verbosity minimal

RUN echo "Europe/London" > /etc/timezone
RUN dpkg-reconfigure -f noninteractive tzdata

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS permissions-api
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Permissions.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS permissions-processor
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Permissions.Processor.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS payments-api
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Payments.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS payments-processor
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Payments.Processor.dll"]


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS management-api
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Management.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS management-processor
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Management.Processor.dll"]


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS reservations-api
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Reservations.Api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS reservations-processor
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Reservations.Processor.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS bff
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Bff.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS ids
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Ids.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS signalr
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Signalr.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS scheduler
WORKDIR /app
COPY --from=build /sln/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Highstreetly.Scheduler.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS logs-cleaner
WORKDIR /app
COPY --from=build /sln/out ./
ENTRYPOINT ["dotnet", "DeleteLogs.dll"]