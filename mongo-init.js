db = db.getSiblingDB('MotorcycleRentDB');

db.createCollection('DeliveryPerson');
db.createCollection('Motorcycle');
db.createCollection('MotorcycleHistory');
db.createCollection('Rent');
db.createCollection('RentalPlan');

db.RentalPlan.insertMany([
    {
        _id: ObjectId("68c2d3c62ebfbc626904ed63"),
        Plano: 7,
        Dias: 7,
        ValorDiaria: 30,
        Multa: 0.2,
        ValorDiariaAposTermino: 50
    },
    {
        _id: ObjectId("68c2d5032ebfbc626904ed64"),
        Plano: 15,
        Dias: 15,
        ValorDiaria: 28,
        Multa: 0.4,
        ValorDiariaAposTermino: 50
    },
    {
        _id: ObjectId("68c2d52b2ebfbc626904ed65"),
        Plano: 30,
        Dias: 30,
        ValorDiaria: 22,
        Multa: 0.4,
        ValorDiariaAposTermino: 50
    },
    {
        _id: ObjectId("68c2d53f2ebfbc626904ed66"),
        Plano: 45,
        Dias: 45,
        ValorDiaria: 20,
        Multa: 0.4,
        ValorDiariaAposTermino: 50
    },
    {
        _id: ObjectId("68c2d5602ebfbc626904ed68"),
        Plano: 50,
        Dias: 50,
        ValorDiaria: 18,
        Multa: 0.4,
        ValorDiariaAposTermino: 50
    }
]);