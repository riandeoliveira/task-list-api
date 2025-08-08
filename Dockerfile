FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

ENV TZ=America/Sao_Paulo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

WORKDIR /app

COPY . ./

RUN dotnet publish -c Release -o out

COPY .env ./out/.env

WORKDIR /app/out

CMD ["dotnet", "ef", "database", "update"]

ENTRYPOINT ["dotnet", "TodoList.dll"]
