STARTUP_PROJECT = src/TodoList
PROJECT_FILE = $(STARTUP_PROJECT)/TodoList.csproj

add-migration:
	@read -p "Migration name: " migration; \
	dotnet ef migrations add $$migration \
		--project $(PROJECT_FILE) \
		--startup-project $(STARTUP_PROJECT)

migrate:
	@dotnet ef database update \
		--project $(PROJECT_FILE) \
		--startup-project $(STARTUP_PROJECT)

add-package:
	@read -p "Package name: " package; \
	dotnet add "$(PROJECT_FILE)" package $$package

build:
	@dotnet build $(PROJECT_FILE)

remove-package:
	@read -p "Package name: " package; \
	dotnet remove "$(PROJECT_FILE)" package $$package

run:
	@dotnet run --project $(STARTUP_PROJECT)
