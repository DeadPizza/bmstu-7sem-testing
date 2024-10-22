generate-allure-report:
	rm -rf allure-reports
	allure generate allure-results -o allure-reports
	allure serve allure-results -p 10000

unit-tests:
	dotnet test Tests/BasedGram.Tests.Unit --no-build --no-restore

integration-tests:
	docker compose up -d
	dotnet test Tests/BasedGram.Tests.Integration --no-build --no-restore
	docker compose down

e2e-tests:
	docker compose up -d
	dotnet test --filter "FullyQualifiedName~E2ETests" --no-build --no-restore
	docker compose down

concat-reports:
	mkdir allure-results
	cp Tests/unit-allure-results/* allure-results/
	cp Tests/integration-allure-results/* allure-results/
	# cp Tests/e2e-allure-results/* allure-results/

.PHONY:
	generate-allure-report unit-tests integration-tests e2e-tests concat-reports