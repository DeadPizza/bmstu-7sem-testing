generate-allure-report:
	rm -rf allure-reports
	allure generate allure-results -o allure-reports
	allure serve allure-results -p 10000

unit-tests:
	dotnet test Tests/BasedGram.Tests.Unit

integration-tests:
	sudo docker compose up -d
	dotnet test Tests/BasedGram.Tests.Integration
	sudo docker compose down

e2e-tests:
	docker compose up -d
	dotnet test Tests/BasedGram.Tests.E2E
	dotnet test E2E/
	docker compose down

concat-reports:
	mkdir allure-results
	cp Tests/unit-allure-results/* allure-results/
	cp Tests/integration-allure-results/* allure-results/
	cp Tests/e2e-allure-results/* allure-results/

static-analysis:
	dotnet run --project HalsteadCompAnalyzer/ BasedGram.WebUI/

.PHONY:
	generate-allure-report unit-tests integration-tests e2e-tests concat-reports static-analysis