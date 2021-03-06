﻿<?php
define('IN_HIGHWAY', true);

require_once('../../../data/config.php');
require_once('../../include/admin_common.inc.php');
require_once 'JSON.php';
//文件保存目录路径
$save_path = '../../'.$upfiles_dir.date("Y/m/d/");
make_dir($save_path);
//文件保存目录URL
$save_url = $_CFG['upfiles_dir'].date("Y/m/d/");
//定义允许上传的文件扩展名
$ext_arr = array(
	'image' => array('gif', 'jpg', 'jpeg', 'png', 'bmp'),
	'flash' => array('swf', 'flv'),
	'media' => array('swf', 'flv', 'mp3', 'wav', 'wma', 'wmv', 'mid', 'avi', 'mpg', 'asf', 'rm', 'rmvb'),
	'file' => array('doc', 'docx', 'xls', 'xlsx', 'ppt', 'htm', 'html', 'txt', 'zip', 'rar', 'gz', 'bz2'),
);
//最大文件大小
$max_size = 100000000;

$save_path = realpath($save_path) . '/';

//PHP上传失败
if (!empty($_FILES['imgFile']['error'])) {
	switch($_FILES['imgFile']['error']){
		case '1':
			$error = 'php.iniに設定のサイズを超えました。';
			break;
		case '2':
			$error = 'テーブルフィールド設定サイズ超えました。';
			break;
		case '3':
			$error = '画像部分アップロードしました。';
			break;
		case '4':
			$error = '画像を選択してください。';
			break;
		case '6':
			$error = '一時フォルダー見つかりません。';
			break;
		case '7':
			$error = 'ファイル書き込むエラー。';
			break;
		case '8':
			$error = 'File upload stopped by extension。';
			break;
		case '999':
		default:
			$error = '未知エラー。';
	}
	alert($error);
}

//有上传文件时
if (empty($_FILES) === false) {
	//原文件名
	$file_name = $_FILES['imgFile']['name'];
	//服务器上临时文件名
	$tmp_name = $_FILES['imgFile']['tmp_name'];
	//文件大小
	$file_size = $_FILES['imgFile']['size'];
	//检查文件名
	if (!$file_name) {
		alert("ファイルを選択してください。");
	}
	//检查目录
	if (@is_dir($save_path) === false) {
		alert("アップロードフォルダーが存在しません。");
	}
	//检查目录写权限
	if (@is_writable($save_path) === false) {
		alert("アップロードフォルダー書けない。");
	}
	//检查是否已上传
	if (@is_uploaded_file($tmp_name) === false) {
		alert("アップロード失敗。");
	}
	//检查文件大小
	if ($file_size > $max_size) {
		alert("アップロードファイルサイズが超えた。");
	}
	//检查目录名
	$dir_name = empty($_GET['dir']) ? 'image' : trim($_GET['dir']);
	if (empty($ext_arr[$dir_name])) {
		alert("フォルダー名不正。");
	}
	//获得文件扩展名
	$temp_arr = explode(".", $file_name);
	$file_ext = array_pop($temp_arr);
	$file_ext = trim($file_ext);
	$file_ext = strtolower($file_ext);
	//检查扩展名
	if (in_array($file_ext, $ext_arr[$dir_name]) === false) {
		alert("アップロードファイル拡張子禁止。\n下記のみ" . implode(",", $ext_arr[$dir_name]) . "フォーマット。");
	}
	//创建文件夹
	if ($dir_name !== '') {
		$save_path .= $dir_name . "/";
		$save_url .= $dir_name . "/";
		if (!file_exists($save_path)) {
			mkdir($save_path);
		}
	}
	$ymd = date("Ymd");
	$save_path .= $ymd . "/";
	$save_url .= $ymd . "/";
	if (!file_exists($save_path)) {
		mkdir($save_path);
	}
	//新文件名
	$new_file_name = date("YmdHis") . '_' . rand(10000, 99999) . '.' . $file_ext;
	//移动文件
	$file_path = $save_path . $new_file_name;
	if (move_uploaded_file($tmp_name, $file_path) === false) {
		alert("アップロードファイル失敗。");
	}
	@chmod($file_path, 0644);
	$file_url = $save_url . $new_file_name;

	header('Content-type: text/html; charset=UTF-8');
	$json = new Services_JSON();
	echo $json->encode(array('error' => 0, 'url' => $file_url));
	exit;
}

function alert($msg) {
	header('Content-type: text/html; charset=UTF-8');
	$json = new Services_JSON();
	echo $json->encode(array('error' => 1, 'message' => $msg));
	exit;
}
