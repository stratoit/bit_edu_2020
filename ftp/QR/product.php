<?php
$serverName = "20.41.81.89";
$connectionOptions = array(
    "database" => "mb_store", // 데이터베이스명
    "uid" => "bit",   // 유저 아이디
    "pwd" => "protec21"    // 유저 비번
);

$제품명 = "Galaxy Note 20";

$출시일 = "2020-07-09";

$CPU = "Octa - core 2.8 GHz";

$크기 = "5.8";

$배터리 = 4700;

$RAM = 16;

$제조사 = "Samsung";

$카메라 = 12;

$무게 = 220;

$가격 = 2200000;

$디스플레이 = "2960x1440";

$메모리 = 256;

// DB커넥션 연결
$dbconn = sqlsrv_connect($serverName, $connectionOptions); 

// 쿼리

$query = "INSERT into product values (제품명,출시일,CPU,크기,배터리,RAM,제조사,카메라,무게,가격,디스플레이,메모리)"; 

// 쿼리를 실행하여 statement 를 얻어온다
//$query = "SELECT name, age FROM test"; 


//$stmt = sqlsrv_query($dbconn, $query);

 

// statement 를 돌면서 필드값을 가져온다

//while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC))

//{

//    echo $row['name'];

//    echo $row['age'];

//    echo "<br>";

//}

 

// 데이터 출력후 statement 를 해제한다

sqlsrv_free_stmt($stmt);

 

// 데이터베이스 접속을 해제한다

sqlsrv_close($dbconn);

?>