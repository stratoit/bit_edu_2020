<?php
$serverName = "20.41.81.89";
$connectionOptions = array(
    "database" => "mb_store", // �����ͺ��̽���
    "uid" => "bit",   // ���� ���̵�
    "pwd" => "protec21"    // ���� ���
);

$��ǰ�� = "Galaxy Note 20";

$����� = "2020-07-09";

$CPU = "Octa - core 2.8 GHz";

$ũ�� = "5.8";

$���͸� = 4700;

$RAM = 16;

$������ = "Samsung";

$ī�޶� = 12;

$���� = 220;

$���� = 2200000;

$���÷��� = "2960x1440";

$�޸� = 256;

// DBĿ�ؼ� ����
$dbconn = sqlsrv_connect($serverName, $connectionOptions); 

// ����

$query = "INSERT into product values (��ǰ��,�����,CPU,ũ��,���͸�,RAM,������,ī�޶�,����,����,���÷���,�޸�)"; 

// ������ �����Ͽ� statement �� ���´�
//$query = "SELECT name, age FROM test"; 


//$stmt = sqlsrv_query($dbconn, $query);

 

// statement �� ���鼭 �ʵ尪�� �����´�

//while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC))

//{

//    echo $row['name'];

//    echo $row['age'];

//    echo "<br>";

//}

 

// ������ ����� statement �� �����Ѵ�

sqlsrv_free_stmt($stmt);

 

// �����ͺ��̽� ������ �����Ѵ�

sqlsrv_close($dbconn);

?>