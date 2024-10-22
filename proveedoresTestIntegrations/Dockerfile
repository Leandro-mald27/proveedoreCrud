FROM postman/newman:5.3.1-alpine as base
WORKDIR /etc/newman

RUN npm install -g newman-reporter-html
RUN npm install -g newman-reporter-json-summary

# Ajustar la ruta de copia
COPY . .

ENTRYPOINT ["newman", "run", "Integration testing basics.postman_collection.json", "--environment=Integrations-environment.postman_environment.json", "--reporters", "cli,json", "--reporter-json-export", "/data/results/outputfile.json"]
