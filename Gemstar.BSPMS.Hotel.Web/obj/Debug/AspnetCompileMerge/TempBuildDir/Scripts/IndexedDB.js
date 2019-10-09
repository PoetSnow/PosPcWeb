//参考资料：https://segmentfault.com/a/1190000002416903
//参考资料：https://blog.csdn.net/rdj_miss/article/details/51285097
//参考资料：https://developer.mozilla.org/zh-CN/docs/Web/API/indexedDB_API/Using_indexedDB

//定义IndexedDB数据库对象
var indexDBObj = {
    name: "indexDBObj",   //数据库名称
    version: 1,           //数据库的版本
    db: null              //数据对象
};

//判断是否支持IndexedDB
function isIndexDB() {
    if (indexedDB) {
        return true;
    }
    else {
        return false;
    }
}

//初始化数据库
function indexDBInit(dbObj) {
    dbObj.version = dbObj.version || 1;
    //打开或创建数据库
    var request = indexedDB.open(dbObj.name, dbObj.version);    //参数为：数据库名和版本号；数据库存在，则打开它；否则创建。
    //指定操作成功的处理函数(可以获得对象存储空间信息)
    request.onsuccess = function (e) {
        dbObj.db = e.target.result;
        //var len = dbObj.db.objectStoreNames.length;   //对象存储空间名的个数
        //var name = dbObj.db.objectStoreNames[i];      //对象存储空间名
    };
    //指定操作失败的处理函数
    request.onerror = function (e) { };
    //onupgradeneeded事件在下列情况下被触发：数据库第一次被打开时；打开数据库时指定的版本号高于当前被持久化的数据库版本号。(可通过修改版本号触发该事件)
    request.onupgradeneeded = function (e) {
        var thisDB = e.target.result;

        //添加数据对象
        if (!thisDB.objectStoreNames.contains("item")) {//判断对象存储空间名称是否已经存在
            //创建 item 对象（表）存储空间，指定keyPath选项为row，unique：属性是否唯一，autoIncrement：是否自动增长
            var objStore = thisDB.createObjectStore("item", { keyPath: "row", autoIncrement: true }); //IndexedDB 自动增长列（每个对象都必须设置，否则将无法进行分页查询）
            objStore.createIndex("Id", "Id", { unique: true }); //在对象存储空间 item 的列 Id 上创建一个唯一索引 Id，可以创建多个索引。
        }
    };
}

//关闭数据库
function indexDBClose(dbObj) {
    dbObj.db.close();
}

//删除数据库
function indexDBDelete(dbObj) {
    indexedDB.deleteDatabase(dbObj.name);
}

//增加数据
function indexDBAddData(dbObj, tableName, data, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readwrite');
    transaction.oncomplete = function () {
        console.log("增加事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };

    var objectStore = transaction.objectStore(tableName);
    var request = objectStore.add(data);
    request.onsuccess = function (e) {
        if (callback) {
            callback({
                error: 0,
                data: data
            })
        }
    };
    request.onerror = function (e) {
        if (callback) {
            callback({
                error: 1
            })
        }
    }
}

//删除数据
function indexDBDeleteData(dbObj, tableName, id, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readwrite');
    transaction.oncomplete = function () {
        console.log("删除事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };
    var objectStore = transaction.objectStore(tableName);
    var request = objectStore.delete(parseInt(id));
    request.onsuccess = function (e) {
        if (callback) {
            callback({
                error: 0,
                data: parseInt(id)
            })
        }
    };
    request.onerror = function (e) {
        if (callback) {
            callback({
                error: 1
            })
        }
    }
}

//更新数据
function indexDBUpdateData(dbObj, tableName, id, updateData, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readwrite');
    transaction.oncomplete = function () {
        console.log("更新事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };

    var objectStore = transaction.objectStore(tableName);
    var request = objectStore.get(id);
    request.onsuccess = function (e) {
        var thisDB = e.target.result;
        for (key in updateData) {
            thisDB[key] = updateData[key];
        }
        objectStore.put(thisDB);
        if (callback) {
            callback({
                error: 0,
                data: thisDB
            })
        }
    };
    request.onerror = function (e) {
        if (callback) {
            callback({
                error: 1
            })
        }
    }
}

//获取全部数据
function indexDBGetAllData(dbObj, tableName, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readonly');
    transaction.oncomplete = function () {
        console.log("查询全部事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };

    var objectStore = transaction.objectStore(tableName);
    var rowData = [];
    objectStore.openCursor(IDBKeyRange.lowerBound(0)).onsuccess = function (event) {
        var cursor = event.target.result;
        if (!cursor && callback) {
            callback({
                error: 0,
                data: rowData
            });
            return;
        }
        rowData.push(cursor.value);
        cursor.continue();
    };
    
}

//根据 ID 获取数据
function indexDBGetDataById(dbObj, tableName, id, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readwrite');
    transaction.oncomplete = function () {
        console.log("查询ID事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };

    var objectStore = transaction.objectStore(tableName);
    var request = objectStore.get(id);
    request.onsuccess = function (e) {
        if (callback) {
            callback({
                error: 0,
                data: e.target.result
            })
        }
    };
    request.onerror = function (e) {
        if (callback) {
            callback({
                error: 1
            })
        }
    }
}

//根据关键词索引获取数据
function indexDBGetDataBySearch(dbObj, tableName, indexName,keywords, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readwrite');
    transaction.oncomplete = function () {
        console.log("查询关键词事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };

    var objectStore = transaction.objectStore(tableName);
    var boundKeyRange = IDBKeyRange.only(keywords);
    var rowData;
    objectStore.index(indexName).openCursor(boundKeyRange).onsuccess = function (event) {
        var cursor = event.target.result;
        if (!cursor) {
            if (callback) {
                callback({
                    error: 0,
                    data: rowData
                })
            }
            return;
        }
        rowData = cursor.value;
        cursor.continue();
    };
}

//根据页码获取数据
function indexDBGetDataByPager(dbObj, tableName, start, end, callback) {
    var transaction = dbObj.db.transaction(tableName, 'readwrite');
    transaction.oncomplete = function () {
        console.log("查询分页事务完成");
    };
    transaction.onerror = function (event) {
        //console.dir(event);
    };

    var objectStore = transaction.objectStore(tableName);
    var boundKeyRange = IDBKeyRange.bound(start, end, false, true);
    var rowData = [];
    objectStore.openCursor(boundKeyRange).onsuccess = function (event) {
        var cursor = event.target.result;
        if (!cursor && callback) {
            callback({
                error: 0,
                data: rowData
            });
            return;
        }
        rowData.push(cursor.value);
        cursor.continue();
    };
}


