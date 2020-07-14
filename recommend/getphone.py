import tensorflow.compat.v1 as tf
import numpy as np
import pymssql
import sys
import os
import time



############데이터 가져오기
conn = pymssql.connect(server='20.41.81.89', user='bit', password='protec21', database='mb_store')
cursor = conn.cursor()
cursor.execute("SELECT sp.product_id FROM sales_product sp "
               "JOIN sales_history sh ON sp.sales_history_id = sh.sales_history_id "
               "JOIN customer c ON sh.customer_id = c.customer_id "
               "WHERE sh.refunded = 0")

row = cursor.fetchone()
dbSet = set()
i=0
while row:
    dbSet.add(row[0])
    row = cursor.fetchone()

conn.close()

dbSetList = list(dbSet)
dbDict = dict()
for i in range(len(dbSetList)):
    dbDict[dbSetList[i]] = i

nb_classes = len(dbSet)  # 결과 종류

X = tf.placeholder(tf.float32, [None, 3])
Y = tf.placeholder(tf.int32, [None, 1])
Y_one_hot = tf.one_hot(Y, nb_classes)  # one hot
Y_one_hot = tf.reshape(Y_one_hot, [-1, nb_classes])

W = tf.Variable(tf.random_normal([3, nb_classes]), name='weight')
b = tf.Variable(tf.random_normal([nb_classes]), name='bias')

logits = tf.matmul(X, W) + b
hypothesis = tf.nn.softmax(logits)

saver = tf.train.Saver()
model = tf.global_variables_initializer()

input_path = "C:/Users/bit/Desktop/bit_project/recommend/recommend_input.txt"

while True:
    while True:
        if(os.path.isfile(input_path)):
            time.sleep(0.1)
            f = open(input_path)
            break
    input_data = list()
    while True:
        line = f.readline()
        if not line: break
        input_data.append(line)
    f.close()
    input_data.append(0)
    # input_data = [0 for i in range(3)]
    # input_data[0] = int(input('나이: '))
    # input_data[1] = int(input('성별(남1, 여0): '))
    # input_data[2] = int(input('개월: '))
    x_data = np.array([input_data])

    with tf.Session() as sess:
        sess.run(model)
        save_path = "./recom.cpkt"
        saver.restore(sess,save_path)

        prediction = tf.argmax(hypothesis, 1)
        dict = sess.run(prediction,feed_dict={X:x_data})
        # print(dict[0])
        idx = str(dict[0])
        for phone in dbSet:
            if dict[0] == dbDict[phone]:
                f = open('C:/Users/bit/Desktop/bit_project/recommend/recommend_result.txt', 'w')
                f.write(str(phone))
                f.close()
                os.remove(input_path)
                break
