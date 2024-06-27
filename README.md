## Projektstruktur

Projektet består af en frontend (https://github.com/it-minds/itm_cvi-lab) og en backend (dette projekt), hvor stacken primært er farvet af, at det startede som et projekt til at skabe overblik over sammenhængen mellem specifikke teknologier. 

Projektet bunder dog i en stack, som fint understøtter det simple krav om, at man som designer nemt skal kunne tilføje nye resourcer, som kan blive vist på en statisk hjemmeside, med høj responstid og nem UX, så man hurtigt og nemt kan finde de fornødne resourcer til sit projekt.

Stacken er:

- Backend: **.NET 8**
- Backoffice: **Umbraco 14.0.0**
- Index: **Elasticsearch**/**Kibana** (containerized)
- Frontend: **NextJS**

## Opsætning

### Frontend

Projektet er sat op helt klassisk, og er derfor selvfølgelig bare bootstrapped med [`create-next-app`](https://github.com/vercel/next.js/tree/canary/packages/create-next-app), så for at komme i gang starter man bare development serveren med:

```bash
npm run dev
# or
yarn dev
# or
pnpm dev
# or
bun dev
```

Herefter kan man åbne løsningen i sin browser på [http://localhost:3000](http://localhost:3000). Det er dog vigtigt at nævne, at frontenden er afhængig af, at Elasticsearch containeren kører, da den får alle resourcer gennem Elasticsearch ved load.

### Backend

Da der ikke på nuværende tidspunkt er en docker-compose til projektet er opsætningen af containers gjort manuelt. Dette skyldes, opsætningen af Kibana, som bruges til at skabe overblik over sit index, kræver en key fra Elasticsearch som først genereres, når Elasticsearch containeren sættes op. Derfor er opsætning af dette følgende:

**Opsætning af Elasticsearch**:

1. Kør kommandoen `docker network create elastic`
2. Kør kommandoen `docker pull docker.elastic.co/elasticsearch/elasticsearch:8.13.4`
3. Kør kommandoen `docker run --name elasticsearch --net elastic -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" -t docker.elastic.co/elasticsearch/elasticsearch:8.13.4`

**Opsætning af Kibana**:

4. Kør kommandoen `docker pull docker.elastic.co/kibana/kibana:8.13.4`
5. Kør kommandoen `docker run --name kibana --net elastic -p 5601:5601 docker.elastic.co/kibana/kibana:8.13.4`

**Sammenkobling af Elasticsearch og Kibana**

6. Tilgå `http://localhost:5601/`
7. Indsæt **enrollment token** fra log i Elasticsearch containeren.
8. Indsæt **verification code** fra log i Kibana containeren.
9. Brug `elastic` som brugernavn og kodeord fra log i Elasticsearch containeren.
10. Tilføj ovenstående til SearchService.tsx, categories/route.tsx, search/route.tsx og ElasticsearchIndexService.cs.
