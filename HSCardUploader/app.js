var dir = require('app-root-path').path;
var _ = require('lodash');
var _getAllFilesFromFolder = function (dir) {

    var filesystem = require("fs");
    var results = [];
    if (dir.indexOf('node_modules') === -1 && dir.indexOf('views') === -1) {
        filesystem.readdirSync(dir).forEach(function (file) {

            file = dir + '/' + file;
            var stat = filesystem.statSync(file);

            if (stat && stat.isDirectory()) {

                results = results.concat(_getAllFilesFromFolder(file))
            } else if (file.indexOf('app.js') === -1 && file.indexOf('package.json') === -1 && file.indexOf('info.txt') === -1) {
                results.push(file.split("/")[1]);
            }


        });
    }

    return results;

};
var files = _getAllFilesFromFolder(dir);
console.log(files);

var cards = [];
fs = require('fs');
fs.readFile(files[0], 'utf8', function (err, data) {
    if (err) {
        return console.log(err);
    }

    var MongoClient = require('mongodb').MongoClient;

    // Connect to the db
    MongoClient.connect("mongodb://localhost:27017/HSDb", function (err, db) {
        if (err) { return console.dir(err); }
        cards = JSON.parse(data);
		
        //  console.log(_.filter(cards, function(o) { return o.type === "HERO"; }));
        cards = _.filter(cards, function (o) { return o.type !== "HERO" && o.id !== "PlaceholderCard"; });
        cards.map(function(elm) {
            elm.cardId = elm.id;
			try{
				elm.name = elm.name.toUpperCase();	
			}
			catch(e){
				console.log(elm.cardId);
			}
            delete elm.id;
        })
        //  console.log(cards.length + ' after');
        var cardsCollection = db.collection('Cards');

         cards.forEach(function(elm, index){
           cardsCollection.insert(elm);
          // console.log(elm)
         });
         
         cardsCollection.count({}, function (error, numOfDocs) {
             console.log('I have ' + numOfDocs + ' documents in my collection');
             // ..
         });
        console.log('finished')
    });
});