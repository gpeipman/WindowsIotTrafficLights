<?php
error_reporting(E_ALL);

header('Content-type: application/json');

$num = rand(0 , 1);
$file = $num == 0 ? 'yellow.json' : 'traffic.json';

echo file_get_contents($file);

?>